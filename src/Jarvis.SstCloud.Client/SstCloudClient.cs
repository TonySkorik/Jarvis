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

		private string _key;
		private string _csrfToken;
		private string _sessionId; 

		#endregion

		#region Props

		public bool IsLoggedIn { private set; get; }

		#endregion

		#region Ctor

		public SstCloudClient(SstCloudSettings settings, in CancellationToken cancellationToken, ILogger logger)
		{
			_settings = settings;
			_cancellationToken = cancellationToken;
			_client = new RestClient(_settings.Uri);
		}

		#endregion

		#region API Methods

		public async Task<bool> LogInAsync()
		{
			IsLoggedIn = false;
			var request = CreateRequest("/auth/login/", Method.POST, AuthInfo.FromSstSettings(_settings));
			var response = await GetResponse(request);

			_key = response.GetResponseBodyProperty<string>("key");
			IsLoggedIn = true;
			
			return IsLoggedIn;
		}

		public async Task<UserInfo> GetUserInfo()
		{
			var request = CreateRequest("/auth/login/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<UserInfo>();
		}

		public async Task<List<HouseInfo>> GetHouses()
		{
			var request = CreateRequest("/houses/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<List<HouseInfo>>();
		}

		public async Task<HouseInfo> GetHouse(int houseId)
		{
			var request = CreateRequest($"/houses/{houseId}/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<HouseInfo>();
		}

		public async Task<List<WaterCounterInfo>> GetHouseWaterCounters(int houseId)
		{
			var request = CreateRequest($"/houses/{houseId}/counters", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<List<WaterCounterInfo>>();
		}

		#endregion

		#region Service methods

		private async Task<IRestResponse> GetResponse(RestRequest request)
		{
			var response = await _client.ExecuteTaskAsync(request, _cancellationToken);
			ExtractCsrfTokenAndSessionId(response);
			if (response.StatusCode != HttpStatusCode.OK)
			{
				// try log in and retry
				if (await LogInAsync())
				{
					request.UpdateCsrfToken(_csrfToken);
					response = await _client.ExecuteTaskAsync(request, _cancellationToken);
				}
			}

			if (response.StatusCode != HttpStatusCode.OK)
			{
				//TODO: log an error
				//TODO: throw!
			}

			return response;
		}
		
		private RestRequest CreateRequest(string resource, Method method, object body = null, params (string name, object value)[] parameters)
		{
			RestRequest request = new RestRequest(resource)
			{
				Method = method
			};

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.UpdateCsrfToken(_csrfToken);

			if (body != null)
			{
				request.AddJsonBody(body);
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

		private void ExtractCsrfTokenAndSessionId(IRestResponse response)
		{
			if (response?.Cookies != null
				&& response.Cookies.Any())
			{
				_csrfToken = response.GetCookieValue("csrftoken");
				_sessionId = response.GetCookieValue("sessionid");
			}
		} 

		#endregion
	}
}