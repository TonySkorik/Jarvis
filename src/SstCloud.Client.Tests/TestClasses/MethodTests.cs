using FluentAssertions;
using SstCloud.Client;
using SstCloud.Client.Tests.Infrastructure;
using Xunit;

namespace SstCloud.Client.Tests.TestClasses;

public class MethodTests
{
	private readonly SstCloudClient _client;
	private readonly CancellationToken _ct = CancellationToken.None;

	public MethodTests()
	{
		var provider = new FromFileSstCloudSettingsProvider();
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