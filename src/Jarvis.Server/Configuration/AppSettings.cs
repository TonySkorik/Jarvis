using System;
using System.Collections.Specialized;
using System.Security;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Jarvis.Server.Configuration
{
	public partial class AppSettings
	{
		public Settings MainSettings { set; get; }
		public QuartzSettings Quartz { set; get; }
	}
}
