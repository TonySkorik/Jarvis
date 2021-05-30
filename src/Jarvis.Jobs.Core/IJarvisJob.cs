using System.Threading.Tasks;

namespace Jarvis.Jobs.Core
{
	public interface IJarvisJob
	{
		public string CodeName { get; }

		public string Description { get; }

		public Task CheckScheduleMisses(string scheduleCronExpression);
	}
}
