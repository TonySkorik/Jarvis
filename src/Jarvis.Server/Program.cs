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
				.ConfigureServices(s=>s.AddHostedService<WaterCheckerService>())
				.ConfigureWebHostDefaults(
					webBuilder =>
					{
						webBuilder.UseStartup<Startup>();
					});
	}
}
