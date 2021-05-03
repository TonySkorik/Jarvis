using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack;

namespace Jarvis.Server.Model
{
	public record JarvisJobSchedule
	{
		public Type? JobType { get; init; }
		public string? CronExpression { get; init; }

		public void Validate()
		{
			if (JobType == null)
			{
				throw new ArgumentNullException(nameof(JobType));
			}

			if (CronExpression.IsNullOrEmpty())
			{
				throw new ArgumentNullException(nameof(CronExpression));
			}
		}
	}
}
