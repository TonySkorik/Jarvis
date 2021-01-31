using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Jarvis.Server.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jarvis.Server.Controllers
{
	[Route("api/status")]
	[ApiController]
	public class StatusController : ControllerBase
	{
		private readonly AppSettings _settings;
		private readonly ILogger<StatusController> _logger;

		public StatusController(AppSettings settings, ILogger<StatusController> logger)
		{
			_settings = settings;
			_logger = logger;
		}

		[HttpGet]
		public RStatus GetStatus()
		{
			return new(
				Environment.OSVersion.VersionString,
				Environment.ProcessorCount,
				Environment.ProcessId,
				Environment.CommandLine);
		}
	}
}
