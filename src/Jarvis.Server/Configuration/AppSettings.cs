namespace Jarvis.Server.Configuration;

public partial class AppSettings
{
	public HostedApiSettings HostedApi { set; get; }
	public Settings Application { set; get; }
	public QuartzSettings Quartz { set; get; }
}