using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Jarvis.Service.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return Json("");
		}		
	}
}
