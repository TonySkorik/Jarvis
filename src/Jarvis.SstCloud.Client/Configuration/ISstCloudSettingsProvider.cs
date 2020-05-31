using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis.SstCloud.Client.Configuration
{
	public interface ISstCloudSettingsProvider
	{
		public SstCloudSettings GetSettings();
	}
}
