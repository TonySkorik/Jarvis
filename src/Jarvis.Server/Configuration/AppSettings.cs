using System;
using System.Collections.Specialized;
using System.Security;
using Jarvis.Core.Configuration.Validation;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Jarvis.Server.Configuration
{
	public partial class AppSettings: IValidatable
	{
		public HostedApiSettings HostedApi { set; get; } = null!;
		public Settings Application { set; get; } = null!;

		public void Validate()
		{
			throw new NotImplementedException();
		}
	}
}
