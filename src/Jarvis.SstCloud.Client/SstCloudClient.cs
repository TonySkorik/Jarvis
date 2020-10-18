using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.SstCloud.Client.Configuration;
using Jarvis.SstCloud.Client.Helpers;
using Jarvis.SstCloud.Client.Model;
using Jarvis.SstCloud.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Serilog;

namespace Jarvis.SstCloud.Client
{
	public class SstCloudClient
	{
		#region Private

		private readonly SstCloudSettings _settings;
		private readonly RestClient _client;

		#endregion

		#region Ctor

		public SstCloudClient(ISstCloudSettingsProvider settingsProvider)
		{
			_settings = settingsProvider.GetSettings();
			_client = new RestClient(_settings.Uri);
		}

		#endregion

		#region API Methods

		public async Task<string> LogInAsync(CancellationToken cancellationToken)
		{
			var request = CreateRequest("/auth/login/", Method.POST, null, AuthInfo.FromSstSettings(_settings));
			var response = await _client.ExecuteAsync(request, cancellationToken);
			var key = response.GetResponseBodyProperty<string>("key");

			return key;
		}

		public async Task<UserInfo> GetUserInfoAsync(string authToken, CancellationToken cancellationToken)
		{
			var request = CreateRequest("/auth/login/", Method.GET, authToken);
			var response = await GetResponse(request, cancellationToken);
			return response.GetResponseBodyAsObject<UserInfo>();
		}

		public async Task<List<HouseInfo>> GetHousesAsync(string authToken, CancellationToken cancellationToken)
		{
			var request = CreateRequest("/houses/", Method.GET, authToken);
			var response = await GetResponse(request, cancellationToken);
			return response.GetResponseBodyAsObjectList<HouseInfo>();
		}

		public async Task<HouseInfo> GetHouseAsync(int houseId, string authToken, CancellationToken cancellationToken)
		{
			var request = CreateRequest($"/houses/{houseId}/", Method.GET, authToken);
			var response = await GetResponse(request, cancellationToken);
			return response.GetResponseBodyAsObject<HouseInfo>();
		}

		public async Task<List<WaterCounterInfo>> GetHouseWaterCountersAsync(string houseName, string authToken, CancellationToken cancellationToken)
		{
			var housesResponse = await GetHousesAsync(authToken, cancellationToken);
			var houseId = housesResponse
				.FirstOrDefault(h => h.Name.Equals(houseName, StringComparison.InvariantCultureIgnoreCase))?.Id;

			if (!houseId.HasValue)
			{
				throw new InvalidOperationException($"House with name {houseName} not found.");
			}

			var request = CreateRequest($"/houses/{houseId.Value}/counters/", Method.GET, authToken);
			var response = await GetResponse(request, cancellationToken);
			var countersResult = response.GetResponseBodyAsObjectList<WaterCounterInfo>();

			return countersResult;
		}

		#endregion

		#region Service methods

		private async Task<IRestResponse> GetResponse(RestRequest request, CancellationToken cancellationToken)
		{
			var response = await _client.ExecuteAsync(request, cancellationToken);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				//TODO: log an error
				//TODO: throw!
			}

			return response;
		}

		private RestRequest CreateRequest(
			string resource,
			Method method,
			string authToken,
			object body = null,
			params (string name, object value)[] parameters)
		{
			RestRequest request = new RestRequest(resource)
			{
				Method = method
			};

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "*/*");
			request.AddHeader("WWW-Authenticate", "Token");

			if (!string.IsNullOrEmpty(authToken))
			{
				request.AddHeader("Authorization", $"Token {authToken}");
			}

			if (body != null)
			{
				var t = JsonConvert.SerializeObject(body);
				request.AddJsonBody(t);
			}

			if (parameters != null && parameters.Length > 0)
			{
				foreach (var parameter in parameters)
				{
					request.AddParameter(parameter.name, parameter.value);
				}
			}

			return request;
		}

		#endregion
	}
}