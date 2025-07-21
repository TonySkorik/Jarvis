using System.Reflection;
using Autofac;
using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure;
using Jarvis.Server.Infrastructure.Services;
using SstCloud.Client;
using SstCloud.Client.Configuration;
using Serilog;

namespace Jarvis.Server.IoC;

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

		//var quartzProperties = settings.Quartz.ToProperties();

		//MicrosoftDependencyInjectionJobFactory f = new MicrosoftDependencyInjectionJobFactory(, new JobFactoryOptions()
		//{
				
		//});

		//containerBuilder.RegisterInstance(new StdSchedulerFactory(quartzProperties))
		//	.As<ISchedulerFactory>()
		//	.SingleInstance();

		containerBuilder.RegisterType<WaterCheckerJob>()
			.AsSelf()
			.SingleInstance();
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