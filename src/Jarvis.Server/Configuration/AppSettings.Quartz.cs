using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Jarvis.Server.Configuration
{
	public partial class AppSettings
	{
		public class QuartzSettings
		{
			public Scheduler Scheduler { get; set; }

			public ThreadPool ThreadPool { get; set; }

			public Plugin Plugin { get; set; }

			public JobStore JobStore { get; set; }

			public NameValueCollection ToProperties()
			{
				var properties = new NameValueCollection
				{
					["quartz.scheduler.instanceName"] = Scheduler?.InstanceName,
					["quartz.threadPool.type"] = ThreadPool?.Type,
					["quartz.threadPool.threadPriority"] = ThreadPool?.ThreadPriority,
					["quartz.threadPool.threadCount"] = ThreadPool?.ThreadCount.ToString(),
					["quartz.jobStore.type"] = JobStore?.Type,
					//["quartz.plugin.jobInitializer.type"] = Plugin?.JobInitializer?.Type,
					//["quartz.plugin.jobInitializer.fileNames"] = Plugin?.JobInitializer?.FileNames
				};

				return properties;
			}
		}

		public class Scheduler
		{
			public string InstanceName { get; set; }
		}

		public class ThreadPool
		{
			public string Type { get; set; }

			public string ThreadPriority { get; set; }

			public int ThreadCount { get; set; }
		}

		public class Plugin
		{
			public JobInitializer JobInitializer { get; set; }
		}

		public class JobInitializer
		{
			public string Type { get; set; }
			public string FileNames { get; set; }
		}

		public class JobStore
		{
			public string Type { get; set; }
		}
	}
}
