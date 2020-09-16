using System.Linq;
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

		public WaterCheckerJob(SstCloudClient client, AppSettings appSetings, EmailSender emailSender, ILogger logger)
		{
			_client = client;
			_appSetings = appSetings;
			_emailSender = emailSender;
			_logger = logger;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			var authToken = await _client.LogInAsync();
			var results = await _client.GetHouseWaterCountersAsync(_appSetings.Application.SstCloud.HouseId, authToken);
			await _emailSender.SendStatisticsAsync(
				results.First(i => i.IsHotWaterCounter),
				results.First(i => !i.IsHotWaterCounter));
		}
	}
}
