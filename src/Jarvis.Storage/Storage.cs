using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.Core.Configuration;
using Jarvis.Jobs.Core;
using Jarvis.Storage.Model;
using ServiceStack;
using ServiceStack.OrmLite;

namespace Jarvis.Storage
{
	public class Storage
	{
		private readonly OrmLiteConnectionFactory _dbFactory;

		public Storage(IDatabaseSettingsProvider settingsProvider)
		{
			_dbFactory = new OrmLiteConnectionFactory(
				settingsProvider.GetConnectionString(),
				SqliteDialect.Provider);
		}

		public async Task WriteJobRun(IJarvisJob job, DateTime jobRunDateTime)
		{
			using var db = _dbFactory.CreateDbConnection();

			var dataToUpsert = new ScheduledJob(job.CodeName, job.Description, jobRunDateTime.SerializeToString());
			await db.UpdateAddAsync(() => dataToUpsert, j => j.CodeName == job.CodeName);
		}

		public async Task<DateTime?> GetJobLastRunTime(string jobCodeName)
		{
			using var db = _dbFactory.CreateDbConnection();

			var result = await db.SelectAsync<ScheduledJob>(j => j.CodeName == jobCodeName);
			return result == null || result.Count == 0
				? (DateTime?)null
				: result.Single().GetJobLastRunDateTime();
		}
	}
}
