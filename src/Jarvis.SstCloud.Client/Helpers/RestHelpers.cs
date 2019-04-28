using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Jarvis.SstCloud.Client.Helpers
{
	public static class RestHelpers
	{
		public static T GetTypedResponse<T>(this IRestResponse response)
		{
			return JsonConvert.DeserializeObject<T>(response.Content);
		}

		public static T GetResponseBodyProperty<T>(this IRestResponse response, string propertyName)
		{
			var bodyObject = JObject.Parse(response.Content);
			return bodyObject[propertyName].Value<T>();
		}

		public static T GetResponseBodyPropertyAsObject<T>(this IRestResponse response, string propertyName)
		{
			var bodyObject = JObject.Parse(response.Content);
			return bodyObject[propertyName].ToObject<T>();
		}

		public static T GetResponseBodyAsObject<T>(this IRestResponse response)
		{
			var bodyObject = JObject.Parse(response.Content);
			return bodyObject.ToObject<T>();
		}

		public static string GetCookieValue(this IRestResponse response, string cookieName)
		{
			return response.Cookies.FirstOrDefault(c => c.Name == cookieName)?.Value;
		}

		public static void UpdateCsrfToken(this RestRequest request, string token)
		{
			request.AddHeader("X-CSRFToken", token);
		}
	}
}
