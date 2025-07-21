using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ScheduledJobsController : ControllerBase
{
	private readonly AppSettings _settings;
	private readonly ILogger<ScheduledJobsController> _logger;
	private readonly WaterCheckerJob _waterCheckerJob;

	public ScheduledJobsController(AppSettings settings, ILogger<ScheduledJobsController> logger, WaterCheckerJob waterCheckerJob)
	{
		_settings = settings;
		_logger = logger;
		_waterCheckerJob = waterCheckerJob;
	}
		
	[HttpGet("check-water")]
	public async Task<string> ExecuteWaterChecker()
	{
		return await _waterCheckerJob.ExecuteCore();
	}
}