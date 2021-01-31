using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;

namespace Jarvis.Server.Model
{
	public record RCommandLineConfig
	{
		[Option("config-path", Required = false, HelpText = "Path to config file")]
		public string ConfigFolderPath { get; init; }
	}
}
