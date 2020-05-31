using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jarvis.Server.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jarvis.Server.Controllers
{
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
	}
}
