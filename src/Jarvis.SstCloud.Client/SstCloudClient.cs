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
		private readonly CancellationToken _cancellationToken;
		private readonly RestClient _client;

		#endregion

		#region Ctor

		public SstCloudClient(ISstCloudSettingsProvider settingsProvider, in CancellationToken cancellationToken)
		{
			_settings = settingsProvider.GetSettings();
			_cancellationToken = cancellationToken;
			_client = new RestClient(_settings.Uri);
		}

		#endregion

		#region API Methods

		public async Task<string> LogInAsync()
		{
			var request = CreateRequest("/auth/login/", Method.POST, null, AuthInfo.FromSstSettings(_settings));
			var response = await _client.ExecuteAsync(request, _cancellationToken);
			var key = response.GetResponseBodyProperty<string>("key");

			return key;
		}

		public async Task<UserInfo> GetUserInfoAsync(string authToken)
		{
			var request = CreateRequest("/auth/login/", Method.GET, authToken);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<UserInfo>();
		}

		public async Task<List<HouseInfo>> GetHousesAsync(string authToken)
		{
			var request = CreateRequest("/houses/", Method.GET, authToken);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObjectList<HouseInfo>();
		}

		public async Task<HouseInfo> GetHouseAsync(int houseId, string authToken)
		{
			var request = CreateRequest($"/houses/{houseId}/", Method.GET, authToken);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<HouseInfo>();
		}

		public async Task<List<WaterCounterInfo>> GetHouseWaterCountersAsync(int houseId, string authToken)
		{
			var request = CreateRequest($"/houses/{houseId}/counters", Method.GET, authToken);
			var response = await GetResponse(request);
			var countersResult = response.GetResponseBodyAsObjectList<WaterCounterInfo>();

			return countersResult;
		}

		#endregion

		#region Service methods

		private async Task<IRestResponse> GetResponse(RestRequest request)
		{
			var response = await _client.ExecuteAsync(request, _cancellationToken);
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
			request.AddHeader("Accept", "application/json");
			if (!string.IsNullOrEmpty(authToken))
			{
				request.AddHeader("Authorization", $"Token {authToken}");
			}

			if (body != null)
			{
				var t = JsonConvert.SerializeObject(body);
				request.AddJsonBody(t);
			}

			if (parameters != null)
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