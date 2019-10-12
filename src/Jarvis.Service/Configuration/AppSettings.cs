using System;
using System.Collections.Generic;
using System.Text;
using Serilog.Events;

namespace Jarvis.Service.Configuration
{
	public class AppSettings
	{
		public MainSettings Settings { set; get; }
		public SstCloudApiSettings SstCloud { set; get; }
		public LoggingSettings Logging { set; get; }

		public class MainSettings
		{
			public TimeSpan WaterCountersCheckInterval { set; get; }
		}

		public class SstCloudApiSettings
		{
			public string Url { get; set; }
			public string Login { get; set; }
			public string Password { get; set; }

			public int HpuseId { set; get; }
		}

		public class LoggingSettings
		{
			public string LogFilePath { get; set; }
			public LogEventLevel EventLevel { get; set; }
		}
	}
	
}
