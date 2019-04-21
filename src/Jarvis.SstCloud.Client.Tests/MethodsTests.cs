using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Jarvis.SstCloud.Client.Configuration;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Jarvis.SstCloud.Client.Tests
{
	public class MethodsTests
	{
		private readonly SstCloudClient _client;

		public MethodsTests()
		{
			var credentials = JObject.Parse(File.ReadAllText("Credentials.json"));
			_client = new SstCloudClient(
				new SstCloudSettings(
					"http://api.sst-cloud.com/",
					credentials["Email"].Value<string>(),
					credentials["Password"].Value<string>()),
				CancellationToken.None,
				null);
		}

		[Fact]
		public async Task TestLogin()
		{
			var loggedIn = await _client.LogInAsync();
			loggedIn.Should().BeTrue();
		}
	}
}
