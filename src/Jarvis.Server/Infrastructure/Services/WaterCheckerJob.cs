using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.SstCloud.Client;
using Quartz;
using Serilog;

namespace Jarvis.Server.Infrastructure.Services
{
	public class WaterCheckerJob : IJob
	{
		private readonly SstCloudClient _client;
		private readonly AppSettings _appSetings;
		private readonly ILogger _logger;
		private readonly EmailSender _emailSender;
		private readonly CancellationTokenSource _shutdownSwitch;
		private static readonly SemaphoreSlim _mutex = new SemaphoreSlim(1,1);

		public WaterCheckerJob(SstCloudClient client, AppSettings appSetings, EmailSender emailSender, ILogger logger, CancellationTokenSource shutdownSwitch)
		{
			_client = client;
			_appSetings = appSetings;
			_emailSender = emailSender;
			_logger = logger;
			_shutdownSwitch = shutdownSwitch;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			await _mutex.WaitAsync();
			try
			{
				var authToken = await _client.LogInAsync(_shutdownSwitch.Token);
				var results = await _client.GetHouseWaterCountersAsync(
					_appSetings.Application.SstCloud.HouseName,
					authToken,
					_shutdownSwitch.Token);
				await _emailSender.SendStatisticsAsync(
					results.First(i => i.IsHotWaterCounter),
					results.First(i => !i.IsHotWaterCounter));
			}
			finally
			{
				_mutex.Release();
			}
		}
	}
}
