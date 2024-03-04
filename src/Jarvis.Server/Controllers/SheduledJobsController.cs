using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SheduledJobsController : ControllerBase
{
	private readonly AppSettings _settings;
	private readonly ILogger<SheduledJobsController> _logger;
	private readonly WaterCheckerJob _waterCheckerJob;

	public SheduledJobsController(AppSettings settings, ILogger<SheduledJobsController> logger, WaterCheckerJob waterCheckerJob)
	{
		_settings = settings;
		_logger = logger;
		_waterCheckerJob = waterCheckerJob;
	}
		
	[HttpGet("waterchecker")]
	public async Task<string> ExecuteWaterChecker()
	{
		return await _waterCheckerJob.ExecuteCore();
	}
}