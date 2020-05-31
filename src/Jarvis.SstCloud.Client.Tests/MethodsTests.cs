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
			loggedIn.Should().BeTrue();
		}

		[Fact]
		public async Task TestGetHouses()
		{
			await _client.LogInAsync();
			var houses = await _client.GetHousesAsync();
			houses.Count.Should().Be(1);
		}

		[Fact]
		public async Task TestGetWaterCounters()
		{
			await _client.LogInAsync();
			var house = (await _client.GetHousesAsync()).First();
			var houseId = house.Id;
			var countersInfo = await _client.GetHouseWaterCountersAsync(houseId);

			countersInfo.Should().NotBeNullOrEmpty();
			countersInfo.Count.Should().Be(2);
			countersInfo.First().Value.Should().BeGreaterThan(0);
			countersInfo.Last().Value.Should().BeGreaterThan(0);
		}
	}
}
