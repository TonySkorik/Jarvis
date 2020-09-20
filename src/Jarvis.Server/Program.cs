using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jarvis.Server.Infrastructure.Services;
using Jarvis.Server.IoC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Jarvis.Server
{
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
				.UseSerilog(AppBuilder.BuildLogger)
				.UseServiceProviderFactory(
					cntxt => new AutofacServiceProviderFactory(
						(cb) => AppBuilder.BuildContainer(cb, cntxt)
					)
				)
				.ConfigureServices(
					s =>
					{
						s.AddHostedService<WaterCheckerService>();

						s.AddQuartz(q =>
						{
							// handy when part of cluster or you want to otherwise identify multiple schedulers
							q.SchedulerId = "Scheduler-Core";

							// we take this from appsettings.json, just show it's possible
							// q.SchedulerName = "Quartz ASP.NET Core Sample Scheduler";

							// we could leave DI configuration intact and then jobs need to have public no-arg constructor
							// the MS DI is expected to produce transient job instances 
							q.UseMicrosoftDependencyInjectionJobFactory(options =>
							{
								// if we don't have the job in DI, allow fallback to configure via default constructor
								options.AllowDefaultConstructor = true;
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
							var jobKey = new JobKey("awesome job", "awesome group");
							q.AddJob<ExampleJob>(j => j
								.StoreDurably()
								.WithIdentity(jobKey)
								.WithDescription("my awesome job")
							);

							q.AddTrigger(t => t
								.WithIdentity("Simple Trigger")
								.ForJob(jobKey)
								.StartNow()
								.WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
								.WithDescription("my awesome simple trigger")
							);

						});

						s.AddQuartzServer(options =>
						{
							// when shutting down we want jobs to complete gracefully
							options.WaitForJobsToComplete = true;
						});
					})
				.ConfigureWebHostDefaults(
					webBuilder =>
					{
						webBuilder.UseStartup<Startup>();
					});
	}
}
