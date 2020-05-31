using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure;
using Jarvis.Server.Infrastructure.Services;
using Jarvis.SstCloud.Client;
using Jarvis.SstCloud.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Serilog;

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
			
			//NameValueCollection props = new NameValueCollection
			//{
			//	{ "quartz.serializer.type", "binary" },
			//	{ "quartz.scheduler.instanceName", "JarvisScheduler" },
			//	{ "quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz" },
			//	{ "quartz.threadPool.threadCount", "16" }
			//};

			var quartzProperties = settings.Quartz.ToProperties();

			containerBuilder.RegisterInstance(new StdSchedulerFactory(quartzProperties))
				.As<ISchedulerFactory>()
				.SingleInstance();

			containerBuilder.RegisterType<WaterCheckerJob>()
				.As<IJob>()
				.SingleInstance();
		}

		public static void BuildLogger(
			HostBuilderContext context,
			LoggerConfiguration loggerConfiguration)
		{
			loggerConfiguration.ReadFrom.Configuration(context.Configuration)
				.Enrich.WithProperty("ApplicationName", typeof(Program).Assembly.GetName().Name)
				.Enrich.WithProperty("Environment", context.HostingEnvironment);
		}
	}
}
