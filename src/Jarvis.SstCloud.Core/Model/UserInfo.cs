using Newtonsoft.Json;

namespace Jarvis.SstCloud.Core.Model
{
	/// <summary>
	/// Represents SST cloud user information
	/// </summary>
	public class UserInfo
	{
		/// <summary>
		/// Represents SST Cloud user profile information
		/// </summary>
		public class UserProfile : IdentifiableValue
		{
			/*
			 * {
					"pk": 5388,
					"username": "just.skorik@yandex.ru",
					"email": "just.skorik@yandex.ru",
					"profile": {
						"id": 5362,
						"phone": null,
						"first_name": "Tony",
						"last_name": null,
						"language": "en",
						"user": 5388
					}
				}
			*/
			
			[JsonProperty("first_name")]
			public string FirstName { get; set; }
			[JsonProperty("last_name")]
			public string LastName { get; set; }
			[JsonProperty("language")]
			public string LanguageCode { get; set; }
			[JsonProperty("user")]
			public int InternalUserId { get; set; }
		}

		[JsonProperty("username")]
		public string UserName { get; set; }
		[JsonProperty("email")]
		public string Email { get; set; }
		[JsonProperty("profile")]
		public UserProfile Profile { get; set; }
	}
}
