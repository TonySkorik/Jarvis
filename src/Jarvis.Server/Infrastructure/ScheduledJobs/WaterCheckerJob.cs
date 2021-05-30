using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Jobs.Core;
using Jarvis.Server.Configuration;
using Jarvis.SstCloud.Client;
using Polly;
using Quartz;
using Serilog;

namespace Jarvis.Server.Infrastructure.ScheduledJobs
{
	[DisallowConcurrentExecution] // either this or _mutex should be used
	public class WaterCheckerJob : IJarvisJob, IJob
	{
		private readonly SstCloudClient _client;
		private readonly AppSettings _appSetings;
		private readonly ILogger _logger;
		private readonly EmailSender _emailSender;
		private readonly CancellationTokenSource _shutdownSwitch;
		private readonly Storage.Storage _jarvisStorage;
		private static readonly SemaphoreSlim _mutex = new(1,1);
		private readonly AsyncPolicy _retryPolicy;

		public string CodeName => nameof(WaterCheckerJob);

		public string Description => "Monthly water check and send job";

		public WaterCheckerJob(
			SstCloudClient client,
			AppSettings appSetings,
			EmailSender emailSender,
			ILogger logger,
			CancellationTokenSource shutdownSwitch,
			Storage.Storage jarvisStorage)
		{
			_client = client;
			_appSetings = appSetings;
			_emailSender = emailSender;
			_logger = logger;
			_shutdownSwitch = shutdownSwitch;
			_jarvisStorage = jarvisStorage;
			_retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(
				5,
				(retryCount, context) => TimeSpan.FromMinutes(retryCount),
				(exception, retryTimeout, context) => _logger.Warning(
					"Exception happened during WaterCheckerJob operations. Retrying {retryeCount} after {retryTimeout}",
					context.Count,
					retryTimeout));
		}

		public async Task CheckScheduleMisses(string scheduleCronExpression)
		{
			var scheduleExpression = new CronExpression(scheduleCronExpression);
			var nextScheduledRun = scheduleExpression.GetNextValidTimeAfter(DateTimeOffset.Now);
			if (!nextScheduledRun.HasValue)
			{
				_logger.Error(
					"Can't calculate schedule miss for job {jobCodeName} using cron expression {cronExpression} : unable to calculate newxt schedule run which is required for calculations",
					CodeName,
					scheduleCronExpression);
				return;
			}

			var nextRunMonth = nextScheduledRun.Value.Month;
			
			var lastRunTime = await _jarvisStorage.GetJobLastRunTime(CodeName);

			if (lastRunTime == null)
			{
				_logger.Warning(
					"Can't calculate schedule miss for job {jobCodeName} using cron expression {cronExpression} : seems like job didn't run even once - no data in database fro the previous run",
					CodeName,
					scheduleCronExpression);
				return;
			}

			var lastRunMonth = lastRunTime.Value.Month;
			
			if (nextRunMonth - lastRunMonth > 1) // 1 means that last run was in previous month - and all is ok. > 1 means that some runs are missed
			{
				// means that at least one month is missed
				_logger.Information(
					"Missed run detected for job {jobCodeName} using cron expression {cronExpression}. Misssed months : {missedMonthsCount}. Starting task now",
					CodeName,
					scheduleCronExpression,
					nextRunMonth - lastRunMonth);

				await ExecuteCore();
			}
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
							_appSetings.Application!.SstCloud!.HouseName,
							authToken,
							_shutdownSwitch.Token);
						var sentLetter = await _emailSender.SendStatisticsAsync(
							results.First(i => i.IsHotWaterCounter),
							results.First(i => !i.IsHotWaterCounter));
						return sentLetter;
					});

				await _jarvisStorage.WriteJobRun(this, DateTime.Now);

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
