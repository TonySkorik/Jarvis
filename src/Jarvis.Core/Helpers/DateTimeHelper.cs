using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Jarvis.Core.Helpers
{
	public static class DateTimeHelper
	{
		public static (bool IsParseSuccess, DateTime Result) TryParseAsDateTime(this string dateTimeString, DateTimeStyles styles = DateTimeStyles.RoundtripKind) 
			=> DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, styles, out DateTime parsed)
				? (true, parsed)
				: (false, DateTime.MinValue);

		public static string SerializeAsDateTimeString(this DateTime target) => target.ToString("O", CultureInfo.InvariantCulture);
	}
}
