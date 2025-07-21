using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace SstCloud.Client.Helpers;

internal static class RestHelpers
{
	public static T GetTypedResponse<T>(this IRestResponse response)
	{
		return JsonConvert.DeserializeObject<T>(response.Content);
	}

	public static T GetResponseBodyProperty<T>(this IRestResponse response, string propertyName)
	{
		var bodyObject = JObject.Parse(response.Content);
		
		if (!bodyObject.ContainsKey(propertyName))
		{
			throw new KeyNotFoundException($"Property '{propertyName}' is not found in response body.");
		}
		
		return bodyObject[propertyName]!.Value<T>();
	}

	public static T GetResponseBodyAsObject<T>(this IRestResponse response)
	{
		var bodyObject = JObject.Parse(response.Content);
		return bodyObject.ToObject<T>();
	}

	public static List<T> GetResponseBodyAsObjectList<T>(this IRestResponse response)
	{
		var bodyObject = JArray.Parse(response.Content);
		return bodyObject.ToObject<List<T>>();
	}

	public static string GetCookieValue(this IRestResponse response, string cookieName)
	{
		return response.Cookies.FirstOrDefault(c => c.Name == cookieName)?.Value;
	}

	public static void UpdateCsrfToken(this RestRequest request, string token)
	{
		request.AddOrUpdateHeader("X-CSRFToken", token);
	}

	public static void AddOrUpdateHeader(this RestRequest request, string headerName, string headerValue)
	{
		var existingHeader =
			request.Parameters.FirstOrDefault(p => p.Type == ParameterType.HttpHeader && p.Name == headerName);
		if (existingHeader != null)
		{
			request.Parameters.Remove(existingHeader);
		}

		request.AddHeader(headerName, headerValue);
	}
}