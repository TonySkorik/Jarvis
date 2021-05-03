using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Server.Model;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace Jarvis.Server.Infrastructure.Services
{
	public class QuartzSchedulerService : IHostedService
	{
		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IJobFactory _jobFactory;
		private readonly IEnumerable<JarvisJobSchedule> _jobSchedules;

		public IScheduler? Scheduler { get; set; }

		public QuartzSchedulerService(
			ISchedulerFactory schedulerFactory,
			IJobFactory jobFactory,
			IEnumerable<JarvisJobSchedule> jobSchedules)
		{
			_schedulerFactory = schedulerFactory;
			_jobSchedules = jobSchedules;
			_jobFactory = jobFactory;
		}
		
		public async Task StartAsync(CancellationToken cancellationToken)
		{
			Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
			Scheduler.JobFactory = _jobFactory;

			foreach (var jobSchedule in _jobSchedules)
			{
				jobSchedule.Validate();

				var job = CreateJob(jobSchedule);
				var trigger = CreateTrigger(jobSchedule);

				await Scheduler.ScheduleJob(job, trigger, cancellationToken);
			}

			await Scheduler.Start(cancellationToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			if(Scheduler != null)
			{
				await Scheduler.Shutdown(cancellationToken);
			}
		}

		private static IJobDetail CreateJob(JarvisJobSchedule schedule)
		{
			var jobType = schedule.JobType!;
			return JobBuilder
				.Create(jobType)
				.WithIdentity(jobType.FullName!)
				.WithDescription(jobType.Name)
				.Build();
		}

		private static ITrigger CreateTrigger(JarvisJobSchedule schedule)
		{
			return TriggerBuilder
				.Create()
				.WithIdentity($"{schedule.JobType!.FullName}.trigger")
				.WithCronSchedule(schedule.CronExpression!, b=> b.InTimeZone(TimeZoneInfo.Local).WithMisfireHandlingInstructionDoNothing())
				.WithDescription(schedule.CronExpression)
				.Build();
		}
	}
}
