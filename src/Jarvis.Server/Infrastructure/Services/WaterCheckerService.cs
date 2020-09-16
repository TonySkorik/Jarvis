using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;

namespace Jarvis.Server.Infrastructure.Services
{
	internal class WaterCheckerService : IHostedService, IDisposable
	{
		private readonly ISchedulerFactory _schedulerFactory;
		private IScheduler _scheduler;

		public WaterCheckerService(ISchedulerFactory schedulerFactory)
		{
			_schedulerFactory = schedulerFactory;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			_scheduler ??= await _schedulerFactory.GetScheduler(cancellationToken);
			await _scheduler.Start(cancellationToken);

			IJobDetail job = JobBuilder.Create<WaterCheckerJob>()
				.WithDescription("Monthly Jarvis water counter check and send job.")
				.WithIdentity("Jarvis.Checks.WaterCounterCheck", "Jarvis.Checks")
				.StoreDurably()
				.Build();

			await _scheduler.AddJob(job, true, cancellationToken);

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity("Jarvis.Checks.Triggers.MonthlyTrigger", "Jarvis.Checks.Triggers")
				.WithDescription("Monthly (every month at 20th day at 13:00) Jarvis water counter check and send trigger.")
				//.WithCronSchedule("0 0 13 20 1/1 ? *")
				.ForJob(job)
				.StartNow()
				.WithCronSchedule("0/10 0 0 ? * * *", b=> b.InTimeZone(TimeZoneInfo.Local).WithMisfireHandlingInstructionFireAndProceed()) // every 10 seconds - debug purposes only
				.Build();

			var scheduleResult = await _scheduler.ScheduleJob(trigger, cancellationToken);

			//var scheduleResult = await _scheduler.ScheduleJob(job, trigger, cancellationToken);

			//await _scheduler.Start(cancellationToken);
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_scheduler.Shutdown(cancellationToken);
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			try
			{
				_scheduler.Clear().GetAwaiter().GetResult();
				_scheduler.Shutdown();
			}
			catch
			{
				// ignore
			}
		}
	}
}
