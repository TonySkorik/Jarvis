using Autofac.Extensions.DependencyInjection;
using CommandLine;
using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure.Services;
using Jarvis.Server.IoC;
using Jarvis.Server.Model;
using Quartz;
using Serilog;

namespace Jarvis.Server;

public class Program
{
	public static void Main(string[] args)
	{
		try
		{
			using IHost host = CreateHostBuilder(args).Build();
			host.Run();
		}
		catch (Exception ex)
		{
			// Log.Logger will likely be internal type "Serilog.Core.Pipeline.SilentLogger".
			if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
			{
				// Loading configuration or Serilog failed.
				Log.Logger = new LoggerConfiguration()
					.MinimumLevel.Debug()
					.WriteTo.Console()
					.CreateLogger();
			}

			Log.Fatal(ex, "Host terminated unexpectedly");
		}
		finally
		{
			Log.CloseAndFlush();
		}

		CreateHostBuilder(args).Build().Run();
	}

	public static IHostBuilder CreateHostBuilder(string[] args) =>
		Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration(
				conf =>
				{
					// get config from parameter
					if (Parser.Default.ParseArguments<CommandLineConfig>(args) is Parsed<CommandLineConfig>
						parsedArgs)
					{
						if (!string.IsNullOrEmpty(parsedArgs.Value.ConfigFolderPath))
						{
							conf.Sources.Clear();
							string configPath = Path.Combine(parsedArgs.Value.ConfigFolderPath, "appsettings.json");
							conf.AddJsonFile(configPath);

							return;
						}
					}

					// get config from ENV
					var configFolderPathFormEnv = Environment.GetEnvironmentVariable("CONFIG_FOLDER_PATH");
					if (!string.IsNullOrEmpty(configFolderPathFormEnv))
					{
						conf.Sources.Clear();
						string configPath = Path.Combine(configFolderPathFormEnv, "appsettings.json");
						conf.AddJsonFile(configPath);
					}

					// default - get config from appsettings.json in the app binaries folder
				})
			.UseSerilog(AppBuilder.BuildLogger)
			.UseServiceProviderFactory(
				cntxt => new AutofacServiceProviderFactory(
					(cb) => AppBuilder.BuildContainer(cb, cntxt)
				)
			)
			.ConfigureServices(
				s =>
				{
					//s.AddHostedService<WaterCheckerService>();
					s.AddQuartz(q =>
					{
						// handy when part of cluster or you want to otherwise identify multiple schedulers
						q.SchedulerId = "Scheduler-Jarvis-Main";

						// we take this from appsettings.json, just show it's possible
						// q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

						// we could leave DI configuration intact and then jobs need to have public no-arg constructor
						// the MS DI is expected to produce transient job instances 
						q.UseMicrosoftDependencyInjectionJobFactory(options =>
						{
							// if we don't have the job in DI, allow fallback to configure via default constructor
						});

						// or 
						// q.UseMicrosoftDependencyInjectionScopedJobFactory();

						// these are the defaults
						q.UseSimpleTypeLoader();
						q.UseInMemoryStore();
						q.UseDefaultThreadPool(tp =>
						{
							tp.MaxConcurrency = 10;
						});
							
						// configure jobs with code
						var waterCheckerJobKey = new JobKey("Jarvis.Checks.WaterCounterCheck", "Jarvis.Checks");
						q.AddJob<WaterCheckerJob>(j => j
							.StoreDurably()
							.WithIdentity(waterCheckerJobKey)
							.WithDescription("Monthly Jarvis water counter check and send job.")
						);

						q.AddTrigger(t => t
							.WithIdentity("Jarvis.Checks.Triggers.MonthlyTrigger", "Jarvis.Checks.Triggers")
							.ForJob(waterCheckerJobKey)
							//.WithCronSchedule("0/10 * * ? * *", b => b.InTimeZone(TimeZoneInfo.Local).WithMisfireHandlingInstructionIgnoreMisfires()) // every 10 seconds - debug purposes only
							.WithCronSchedule("0 0 12 20 * ? *", b => b.InTimeZone(TimeZoneInfo.Local).WithMisfireHandlingInstructionIgnoreMisfires()) // every 20th day of every month
							.StartNow()
							.WithDescription("Monthly (every month at 20th day at 12:00) Jarvis water counter check and send trigger.")
						);
					});

					s.AddQuartzServer(options =>
					{
						// when shutting down we want jobs to complete gracefully
						options.WaitForJobsToComplete = true;
					});
				})
			.ConfigureWebHostDefaults(
				webBuilder => { 
					webBuilder.UseKestrel(
						(context, opt) =>
						{
							var config = context.Configuration.Get<AppSettings>();
							opt.ListenLocalhost(config.HostedApi.Port);
						});

					webBuilder.UseStartup<Startup>();
				})
			.UseWindowsService()
			.UseSystemd();
}