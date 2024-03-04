using CommandLine;

namespace Jarvis.Server.Model;

internal record CommandLineConfig
{
	[Option("config-path", Required = false, HelpText = "Path to config file")]
	public string ConfigFolderPath { get; init; }
}