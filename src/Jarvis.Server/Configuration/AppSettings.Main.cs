using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Jarvis.Server.Model;
using Serilog.Events;

namespace Jarvis.Server.Configuration
{
	public partial class AppSettings
	{
		public class Settings
		{
			public SstCloudApiSettings? SstCloud { set; get; }
			public LoggingSettings Logging { set; get; } = null!;
			public EmailSettings EmailSender { set; get; } = null!;
			public string JarvisDbConnectionString { set; get; } = null!;
			public List<JarvisJobSchedule> JobSchedules { set; get; } = null!;
		}

		public class EmailSettings
		{
			public string Host { set; get; } = null!;
			public int Port { set; get; }
			public string Login { get; set; } = null!;
			public string Password { get; set; } = null!;
			public string From { set; get; } = null!;
			public string[] To { set; get; } = null!;
			public string[] Bcc { set; get; } = null!;
			public string Subject { set; get; } = null!;
			public string TemplatePath { set; get; } = null!;
			public string[] AdminEmails { set; get; } = null!;
		}

		public class SstCloudApiSettings
		{
			public string Url { get; set; } = null!;
			public string Login { get; set; } = null!;
			public string Password { get; set; } = null!;
			public string HouseName { set; get; } = null!;
		}

		public class LoggingSettings
		{
			public string LogFilePath { get; set; } = null!;
			public LogEventLevel EventLevel { get; set; }
		}
	}
}
