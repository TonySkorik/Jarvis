namespace SstCloud.Client.Configuration;

public class SstCloudSettings
{
	public string Email { get; }
	
	public string Password { get; }
	
	public string Uri { get; }
	
	public TimeSpan Timeout { set; get; }

	public SstCloudSettings(string uri, string email, string password)
	{
		Uri = uri;
		Email = email;
		Password = password;
	}
}