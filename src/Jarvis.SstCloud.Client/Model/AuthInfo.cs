using System;
using System.Collections.Generic;
using System.Text;
using Jarvis.SstCloud.Client.Configuration;
using Newtonsoft.Json;

namespace Jarvis.SstCloud.Client.Model
{
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

		public static AuthInfo FromSstSettings(SstCloudSettings settings)
		{
			return new AuthInfo(settings.Email, settings.Password);
		}
	}
}
