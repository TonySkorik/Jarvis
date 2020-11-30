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
		private readonly CancellationToken _ct = CancellationToken.None;

		public MethodsTests()
		{
			var provider = new DummySstCloudSettingsProvider();
			_client = new SstCloudClient(provider);
		}

		[Fact]
		public async Task TestLogin()
		{
			var authToken = await _client.LogInAsync(_ct);
			authToken.Should().NotBeNullOrEmpty();
		}

		[Fact]
		public async Task TestGetHouses()
		{
			var authToken = await _client.LogInAsync(_ct);
			var houses = await _client.GetHousesAsync(authToken, _ct);
			houses.Count.Should().Be(1);
		}

		[Fact]
		public async Task TestGetWaterCounters()
		{
			var authToken = await _client.LogInAsync(_ct);

			var houses = await _client.GetHousesAsync(authToken, _ct);
			var house = houses.First();
			var houseName = house.Name;
			
			var countersInfo = await _client.GetHouseWaterCountersAsync(houseName, authToken, _ct);

			countersInfo.Should().NotBeNullOrEmpty();
			countersInfo.Count.Should().Be(2);
			countersInfo.First().Value.Should().BeGreaterThan(0);
			countersInfo.Last().Value.Should().BeGreaterThan(0);
		}
	}
}
