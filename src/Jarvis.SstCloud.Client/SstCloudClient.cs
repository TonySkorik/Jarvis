﻿using System;
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

		public async Task<bool> LogIn()
		{
			IsLoggedIn = false;
			var request = CreateRequest("/auth/login/", Method.POST, AuthInfo.FromSstSettings(_settings));
			var response = await _client.ExecuteTaskAsync(request, _cancellationToken);
			ExtractCsrfTokenAndSessionId(response);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				_key = response.GetResponseBodyProperty<string>("key");
				IsLoggedIn = true;
			}

			// TODO: log error
			return IsLoggedIn;
		}

		#endregion

		#region Service methods

		private RestRequest CreateRequest(string resource, Method method, object body = null, params (string name, object value)[] parameters)
		{
			RestRequest request = new RestRequest(resource)
			{
				Method = method
			};

			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("X-CSRFToken", _csrfToken);

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