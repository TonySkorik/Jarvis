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
		public class HostedApiSettings
		{
			public int Port { set; get; }
		}
	}
}
