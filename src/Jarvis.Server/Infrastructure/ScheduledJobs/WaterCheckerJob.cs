﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.Server.Model;
using Jarvis.SstCloud.Client;
using Polly;
using Quartz;
using Serilog;

namespace Jarvis.Server.Infrastructure.ScheduledJobs
{
	[DisallowConcurrentExecution] // either this or _mutex should be used
	public class WaterCheckerJob : IJarvisJob
	{
		private readonly SstCloudClient _client;
		private readonly AppSettings _appSetings;
		private readonly ILogger _logger;
		private readonly EmailSender _emailSender;
		private readonly CancellationTokenSource _shutdownSwitch;
		private static readonly SemaphoreSlim _mutex = new(1,1);
		private readonly AsyncPolicy _retryPolicy;

		public string Name => nameof(WaterCheckerJob);
		
		public WaterCheckerJob(
			SstCloudClient client,
			AppSettings appSetings,
			EmailSender emailSender,
			ILogger logger,
			CancellationTokenSource shutdownSwitch)
		{
			_client = client;
			_appSetings = appSetings;
			_emailSender = emailSender;
			_logger = logger;
			_shutdownSwitch = shutdownSwitch;
			_retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
				5,
				(retryCount, context) => TimeSpan.FromMinutes(retryCount),
				(exception, retryTimeout, context) => _logger.Warning(
					"Exception happened during WaterCheckerJob operations. Retrying {retryeCount} after {retryTimeout}",
					context.Count,
					retryTimeout));
		}

		public Task CheckScheduleMisses()
		{
			throw new NotImplementedException();
		}

		public Task Execute(IJobExecutionContext context)
		{
			return ExecuteCore();
		}

		public async Task<string> ExecuteCore()
		{
			await _mutex.WaitAsync();
			try
			{
				var sent = await _retryPolicy.ExecuteAsync(
					async () =>
					{
						var authToken = await _client.LogInAsync(_shutdownSwitch.Token);
						var results = await _client.GetHouseWaterCountersAsync(
							_appSetings.Application.SstCloud.HouseName,
							authToken,
							_shutdownSwitch.Token);
						var sentLetter = await _emailSender.SendStatisticsAsync(
							results.First(i => i.IsHotWaterCounter),
							results.First(i => !i.IsHotWaterCounter));
						return sentLetter;
					});

				return sent;
			}
			catch (Exception ex)
			{
				var sent = await _retryPolicy.ExecuteAsync(
					async () =>
					{
						_logger.Error(ex, "Exception happened during Jarvis water checker jobs operations");
						var sentLetter = await _emailSender.NotifyAboutJarvisException(ex.ToString());
						return sentLetter;
					});
				
				return sent;
			}
			finally
			{
				_mutex.Release();
			}
		}
	}
}
