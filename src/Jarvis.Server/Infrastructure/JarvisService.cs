using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure.Jobs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Serilog;

namespace Jarvis.Server.Infrastructure
{
	public class JarvisService
	{
		private readonly string[] _args;
		private Task _webServiceTask;
		private readonly CancellationTokenSource _killswitch;
		private readonly AppSettings _config;
		private readonly Task _initialized;

		public ILogger Logger { get; }
		public IScheduler Scheduler { get; private set; }

		public JarvisService(string[] args)
		{
			_config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddUserSecrets<Startup>(true)
				.Build().Get<AppSettings>();

			Log.Logger = new LoggerConfiguration()
				.WriteTo.File(_config.Logging.LogFilePath, _config.Logging.EventLevel)
				.CreateLogger();

			Logger = Log.Logger;

			_killswitch = new CancellationTokenSource();
			_args = args;
			
			_initialized = InitilizeSheduler(_killswitch.Token);
		}

		private async Task InitilizeSheduler(CancellationToken cancellationToken)
		{
			NameValueCollection props = new NameValueCollection
			{
				{ "quartz.serializer.type", "binary" },
				{ "quartz.scheduler.instanceName", "MyScheduler" },
				{ "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
				{ "quartz.threadPool.threadCount", "2" }
			};
			StdSchedulerFactory schedulerFactory = new StdSchedulerFactory(props);
			Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
			await Scheduler.Start(cancellationToken);

			IJobDetail job = JobBuilder.Create<WaterCheckerJob>()
				.SetJobData(
					new JobDataMap(
						(IDictionary<string, object>) new Dictionary<string, object>()
						{
							{"Settings", _config},
							{"CancellationToken", _killswitch.Token},
							{"Logger", Log.Logger},
							{"EmailSender", new EmailSender(_config)}
						}))
				.WithDescription("Monthly Jarvis water counter check and send job.")
				.WithIdentity("Jarvis.WaterCounterCheck", "Jarvis.Checks")
				.Build();

			ITrigger trigger = TriggerBuilder.Create()
				.WithIdentity("Jarvis.MonthlyTrigger", "Jarvis.Checks")
				.WithDescription("Monthly Jarvis water counter check and send trigger.")
				.StartNow()
				.WithCalendarIntervalSchedule(x=>x.WithIntervalInMonths(1))
				.Build();

			await Scheduler.ScheduleJob(job, trigger, cancellationToken);
		}

		public async void Start()
		{
			await _initialized;
			_webServiceTask = CreateWebHostBuilder(_args)
				.Build().RunAsync(_killswitch.Token);
		}

		public void Stop()
		{
			_killswitch.Cancel();
		}

		#region Methods for building web host
		
		private IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureServices(
					srv => { 
						srv.AddSingleton(_killswitch);
						srv.AddSingleton(_config);
					})
				.UseStartup<Startup>(); 

		#endregion
	}
}
