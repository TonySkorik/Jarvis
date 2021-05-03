using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Jarvis.Server.Configuration;
using Jarvis.Server.Diagnostics;
using Jarvis.Server.Infrastructure;
using Jarvis.Server.Infrastructure.ScheduledJobs;
using Jarvis.SstCloud.Client;
using Jarvis.SstCloud.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using Quartz.Simpl;
using Quartz.Spi;
using Serilog;
using Serilog.Extensions.Logging;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace Jarvis.Server.IoC
{
	internal static class AppBuilder
	{
		public static void BuildContainer(ContainerBuilder containerBuilder, HostBuilderContext context)
		{
			var settings = context.Configuration.Get<AppSettings>();
			containerBuilder.RegisterInstance(settings)
				.SingleInstance();
			containerBuilder.RegisterType<SettingsProvider>()
				.As<ISstCloudSettingsProvider>()
				.SingleInstance();
			containerBuilder.RegisterType<SstCloudClient>()
				.InstancePerDependency();
			containerBuilder.RegisterType<EmailSender>()
				.AsSelf()
				.InstancePerDependency();
			containerBuilder.RegisterType<CancellationTokenSource>()
				.AsSelf()
				.SingleInstance();

			containerBuilder.Register<IDbConnectionFactory>(c =>
				new OrmLiteConnectionFactory(settings.Application.JarvisDbConnectionString, SqliteDialect.Provider));
			
			containerBuilder.RegisterType<WaterCheckerJob>()
				.AsSelf()
				.SingleInstance();

			containerBuilder.RegisterType<MicrosoftDependencyInjectionJobFactory>()
				.As<IJobFactory>()
				.SingleInstance();

			containerBuilder.RegisterType<StdSchedulerFactory>()
				.As<ISchedulerFactory>()
				.SingleInstance();

			// register Jarvis job schedules
			foreach (var jobSchedule in settings.Application.JobSchedules)
			{
				containerBuilder.RegisterInstance(jobSchedule).SingleInstance();
			}
		}
		
		public static void BuildLogger(
			HostBuilderContext context,
			LoggerConfiguration loggerConfiguration)
		{
			var appSettings = context.Configuration.Get<AppSettings>();

			var config = loggerConfiguration.ReadFrom.Configuration(context.Configuration, "Application");
			var logFilePath = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
				appSettings.Application.Logging.LogFilePath);

			config
				.MinimumLevel.Is(appSettings.Application.Logging.EventLevel)
				.WriteTo.Console()
				.WriteTo.File(logFilePath, appSettings.Application.Logging.EventLevel)
				.Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
				.Enrich.WithProperty("Environment", context.HostingEnvironment);
		}
	}
}
