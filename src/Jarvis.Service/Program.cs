using System;
using System.Threading.Tasks;
using Jarvis.Service.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Topshelf;

namespace Jarvis.Service
{
	class Program
	{
		public static void Main(string[] args)
		{
			var rc = HostFactory.Run(x =>
			{
				x.Service<JarvisService>(s =>
				{
					s.ConstructUsing(name => new JarvisService(args));
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});
				x.RunAsLocalSystem();

				x.SetDescription("Jarvis background service");
				x.SetDisplayName("Jarvie Backgorund Service");
				x.SetServiceName("Jarvis.Background");
			});
			
			var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
			Environment.ExitCode = exitCode;
		}
	}
}
