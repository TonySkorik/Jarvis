using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Service.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Jarvis.Service.Infrastructure
{
	public class JarvisService
	{
		private readonly string[] _args;
		private Task _webServiceTask;
		private WaterChecker _waterChecker;
		private readonly CancellationTokenSource _killswitch;
		private readonly AppSettings _config;

		public JarvisService(string[] args)
		{
			_config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.AddUserSecrets<Startup>(true)
				.Build().Get<AppSettings>();

			Log.Logger = new LoggerConfiguration()
				.WriteTo.File(_config.Logging.LogFilePath, _config.Logging.EventLevel)
				.CreateLogger();

			_args = args;
			_killswitch = new CancellationTokenSource();
			_waterChecker = new WaterChecker(
				_config.Settings.WaterCountersCheckInterval,
				_config,
				_killswitch.Token,
				Log.Logger);
		}

		public void Start()
		{
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
