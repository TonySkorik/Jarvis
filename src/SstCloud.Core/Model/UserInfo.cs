using Newtonsoft.Json;

namespace SstCloud.Core.Model;

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
				"username": "a@yandex.ru",
				"email": "a@yandex.ru",
				"profile": {
					"id": 5362,
					"phone": null,
					"first_name": "T",
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