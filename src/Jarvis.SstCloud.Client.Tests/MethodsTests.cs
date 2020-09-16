using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
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
			var provider = new DummySstCloudSettingsProvider();
			_client = new SstCloudClient(provider, CancellationToken.None);
		}

		[Fact]
		public async Task TestLogin()
		{
			var loggedIn = await _client.LogInAsync();
			loggedIn.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task TestGetHouses()
		{
			var authToken = await _client.LogInAsync();
			var houses = await _client.GetHousesAsync(authToken);
			houses.Count.Should().Be(1);
		}

		[Fact]
		public async Task TestGetWaterCounters()
		{
			var authToken = await _client.LogInAsync();
			var house = (await _client.GetHousesAsync(authToken)).First();
			var houseId = house.Id;
			var countersInfo = await _client.GetHouseWaterCountersAsync(houseId, authToken);

			countersInfo.Should().NotBeNullOrEmpty();
			countersInfo.Count.Should().Be(2);
			countersInfo.First().Value.Should().BeGreaterThan(0);
			countersInfo.Last().Value.Should().BeGreaterThan(0);
		}
	}
}
