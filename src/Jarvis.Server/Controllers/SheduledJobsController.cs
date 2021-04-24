using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.Server.Infrastructure.ScheduledJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jarvis.Server.Controllers
{
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
}
