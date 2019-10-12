using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Jarvis.Service.Configuration;
using Jarvis.SstCloud.Client;
using Jarvis.SstCloud.Client.Configuration;
using Serilog;

namespace Jarvis.Service.Infrastructure
{
	public class WaterChecker
	{
		private readonly AppSettings _settings;
		private readonly SstCloudClient _client;
		private readonly System.Timers.Timer _checkTimer;

		public WaterChecker(
			TimeSpan checkInterval,
			AppSettings settings,
			CancellationToken cancellationToken,
			ILogger logger)
		{
			_settings = settings;
			_client = new SstCloudClient(
				new SstCloudSettings(settings.SstCloud.Url, settings.SstCloud.Login, settings.SstCloud.Password),
				cancellationToken,
				logger);

			//TODO: use quartz

			_checkTimer = new System.Timers.Timer(checkInterval.TotalMilliseconds);
			_checkTimer.Elapsed += CheckTimerElapsed;
		}

		private async void CheckTimerElapsed(object sender, ElapsedEventArgs e)
		{

			await _client.LogInAsync();
			await _client.GetHouseWaterCountersAsync(_settings.SstCloud.HpuseId);
		}

		public void Run()
		{
			_checkTimer.Start();
		}
	}
}
