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
			var response = await _client.ExecuteTaskAsync(request, _cancellationToken);
			ExtractCsrfTokenAndSessionId(response);

			_key = response.GetResponseBodyProperty<string>("key");
			IsLoggedIn = true;
			
			return IsLoggedIn;
		}

		public async Task<UserInfo> GetUserInfoAsync()
		{
			var request = CreateRequest("/auth/login/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<UserInfo>();
		}

		public async Task<List<HouseInfo>> GetHousesAsync()
		{
			var request = CreateRequest("/houses/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObjectList<HouseInfo>();
		}

		public async Task<HouseInfo> GetHouseAsync(int houseId)
		{
			var request = CreateRequest($"/houses/{houseId}/", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObject<HouseInfo>();
		}

		public async Task<List<WaterCounterInfo>> GetHouseWaterCountersAsync(int houseId)
		{
			var request = CreateRequest($"/houses/{houseId}/counters", Method.GET);
			var response = await GetResponse(request);
			return response.GetResponseBodyAsObjectList<WaterCounterInfo>();
		}

		#endregion

		#region Service methods

		private async Task<IRestResponse> GetResponse(RestRequest request)
		{
			var response = await _client.ExecuteTaskAsync(request, _cancellationToken);
			ExtractCsrfTokenAndSessionId(response);
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

			request.AddCookie("csrftoken", _csrfToken);
			request.AddCookie("sessionid", _sessionId);

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.UpdateCsrfToken(_csrfToken);
			
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