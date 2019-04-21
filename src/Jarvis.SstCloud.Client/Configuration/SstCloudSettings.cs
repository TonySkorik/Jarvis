using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Jarvis.SstCloud.Client.Configuration
{
	public class SstCloudSettings
	{
		public string Email { set; get; }
		public string Password { get; set; }
		public string Uri { get; set; }
		public TimeSpan Timeout { set; get; }
	}
}
