using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Serilog.Events;

namespace Jarvis.Service.Configuration
{
	public class AppSettings
	{
		public MainSettings Settings { set; get; }
		public SstCloudApiSettings SstCloud { set; get; }
		public LoggingSettings Logging { set; get; }
		public EmailSettings EmailSender { set; get; }

		public class EmailSettings
		{
			public string Host { set; get; }
			public int Port { set; get; }
			public string Login { get; set; }
			public SecureString Password { get; set; }
			public string From { set; get; }
			public string[] To { set; get; }
			public string[] Bcc { set; get; }
			public string Subject { set; get; }
			public string TemplatePath { set; get; }
		}

		public class MainSettings
		{
			public TimeSpan WaterCountersCheckInterval { set; get; }
		}

		public class SstCloudApiSettings
		{
			public string Url { get; set; }
			public string Login { get; set; }
			public string Password { get; set; }

			public int HouseId { set; get; }
		}

		public class LoggingSettings
		{
			public string LogFilePath { get; set; }
			public LogEventLevel EventLevel { get; set; }
		}
	}
	
}
