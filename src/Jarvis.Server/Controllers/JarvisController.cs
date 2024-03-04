using Jarvis.Server.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JarvisController : ControllerBase
{
	private readonly AppSettings _settings;
	private readonly ILogger<JarvisController> _logger;

	public JarvisController(AppSettings settings, ILogger<JarvisController> logger)
	{
		_settings = settings;
		_logger = logger;
	}

	[HttpGet("status")]
	public string GetStatus()
	{
		return "OK";
	}
}