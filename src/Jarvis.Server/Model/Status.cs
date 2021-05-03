using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Server.Model
{
	public record Status(string Os, int ProcessorCount, int ProcessId, string CommandLine);
}
