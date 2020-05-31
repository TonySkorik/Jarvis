using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.SstCloud.Client.Configuration;

namespace Jarvis.Server.Configuration
{
	public class SettingsProvider : ISstCloudSettingsProvider
	{
		private readonly AppSettings _settings;

		public SettingsProvider(AppSettings settings)
		{
			_settings = settings;
		}

		SstCloudSettings ISstCloudSettingsProvider.GetSettings()
		{
			return new SstCloudSettings(_settings.SstCloud.Url, _settings.SstCloud.Login, _settings.SstCloud.Password);
		}
	}
}
