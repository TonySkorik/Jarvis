using Newtonsoft.Json;
using SstCloud.Client.Configuration;

namespace SstCloud.Client.Model;

internal class AuthInfo
{
	[JsonProperty("username")]
	public string UserName => Email;
	[JsonProperty("email")]
	public string Email { get; }
	[JsonProperty("password")]
	public string Password { get; }

	public AuthInfo(string email, string password) 
		=> (Email, Password) = (email, password);
		
	public static object FromSstSettings(SstCloudSettings settings)
	{
		return new AuthInfo(settings.Email, settings.Password);
	}
}