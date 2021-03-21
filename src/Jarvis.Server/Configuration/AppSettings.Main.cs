using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Serilog.Events;

namespace Jarvis.Server.Configuration
{
	public partial class AppSettings
	{
		public class Settings
		{
			public SstCloudApiSettings SstCloud { set; get; }
			public LoggingSettings Logging { set; get; }
			public EmailSettings EmailSender { set; get; }
			public string ScheduledJobsExecutionLogsFolder { set; get; } = "ScheduledJobsExecutionLogs";
		}

		public class EmailSettings
		{
			public string Host { set; get; }
			public int Port { set; get; }
			public string Login { get; set; }
			public string Password { get; set; }
			public string From { set; get; }
			public string[] To { set; get; }
			public string[] Bcc { set; get; }
			public string Subject { set; get; }
			public string TemplatePath { set; get; }
			public string[] AdminEmails { set; get; }
		}

		public class SstCloudApiSettings
		{
			public string Url { get; set; }
			public string Login { get; set; }
			public string Password { get; set; }
			public string HouseName { set; get; }
		}

		public class LoggingSettings
		{
			public string LogFilePath { get; set; }
			public LogEventLevel EventLevel { get; set; }
		}
	}
}
