using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jarvis.Core.Helpers;
using ServiceStack.DataAnnotations;

namespace Jarvis.Storage.Model
{
	[Alias("ScheduledJob")]
	[Schema("dbo")]
	internal record ScheduledJob(
		[property: Required] 
		string CodeName,

		string Decription,

		[property: Required] 
		string LastRunDateTime)
	{
		[AutoIncrement]
		public int Id { get; init; }

		public DateTime GetJobLastRunDateTime()
		{
			var parseResult = LastRunDateTime.TryParseAsDateTime();
			if (parseResult.IsParseSuccess)
			{
				return parseResult.Result;
			}

			throw new InvalidOperationException($"Can't parse string {LastRunDateTime} as DateTime.");
		}
	}
}
