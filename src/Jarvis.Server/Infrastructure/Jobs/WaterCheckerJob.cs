using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.SstCloud.Client;
using Jarvis.SstCloud.Client.Configuration;
using Quartz;
using Serilog;

namespace Jarvis.Server.Infrastructure.Jobs
{
	public class WaterCheckerJob : IJob
	{
		public AppSettings Config { get; set; }
		public CancellationToken CancellationToken { get; set; }
		public ILogger Logger { get; set; }
		public EmailSender EmailSender { get; set; }

		public async Task Execute(IJobExecutionContext context)
		{
			var client = new SstCloudClient(
				new SstCloudSettings(Config.SstCloud.Url, Config.SstCloud.Login, Config.SstCloud.Password),
				CancellationToken,
				Logger);

			await client.LogInAsync();
			var results = await client.GetHouseWaterCountersAsync(Config.SstCloud.HouseId);
			await EmailSender.SendStatisticsAsync(
				results.First(i => i.IsHotWaterCounter),
				results.First(i => !i.IsHotWaterCounter));
		}
	}
}
