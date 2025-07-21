using SstCloud.Client.Configuration;
using Newtonsoft.Json.Linq;

namespace SstCloud.Client.Tests.Infrastructure;

internal class FromFileSstCloudSettingsProvider : ISstCloudSettingsProvider
{
	public SstCloudSettings GetSettings()
	{
		var credentials = JObject.Parse(File.ReadAllText("Credentials.json"));
		if (credentials == null)
		{
			throw new ArgumentNullException(nameof(credentials));
		}
		
		if (credentials["Email"] == null || credentials["Password"] == null)
		{
			throw new ArgumentException("Credentials.json must contain both 'Email' and 'Password' properties.");
		}

		var ret = new SstCloudSettings(
			"https://api.sst-cloud.com/",
			credentials["Email"]!.Value<string>(),
			credentials["Password"]!.Value<string>());
		
		return ret;
	}
}