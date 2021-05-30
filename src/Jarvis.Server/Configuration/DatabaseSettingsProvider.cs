using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Core.Configuration;

namespace Jarvis.Server.Configuration
{
	public class DatabaseSettingsProvider : IDatabaseSettingsProvider
	{
		private readonly AppSettings _settings;

		public DatabaseSettingsProvider(AppSettings settings)
		{
			_settings = settings;
		}

		public string GetConnectionString() => _settings.Application.JarvisDbConnectionString;
	}
}
