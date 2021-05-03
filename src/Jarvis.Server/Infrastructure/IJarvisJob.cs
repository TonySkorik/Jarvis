using System.Threading.Tasks;
using Quartz;

namespace Jarvis.Server.Infrastructure
{
	public interface IJarvisJob : IJob
	{
		public string Name { get; }

		public Task CheckScheduleMisses();
	}
}
