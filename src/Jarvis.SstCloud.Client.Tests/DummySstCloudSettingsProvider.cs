﻿using Jarvis.SstCloud.Client.Configuration;
using Newtonsoft.Json.Linq;

namespace Jarvis.SstCloud.Client.Tests;

class DummySstCloudSettingsProvider : ISstCloudSettingsProvider
{
	public SstCloudSettings GetSettings()
	{
		var credentials = JObject.Parse(File.ReadAllText("Credentials.json"));
		if (credentials == null)
		{
			throw new ArgumentNullException(nameof(credentials));
		}

		var ret = new SstCloudSettings(
			"https://api.sst-cloud.com/",
			credentials["Email"]?.Value<string>(),
			credentials["Password"]?.Value<string>());
		return ret;
	}
}