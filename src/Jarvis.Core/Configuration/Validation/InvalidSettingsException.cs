using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Core.Configuration.Validation
{
	class InvalidSettingsException : Exception
	{
		public InvalidSettingsException(IEnumerable<string> invalidSettingMessages) : base(
			$"Propery validation failed with the following errors: {string.Join(";", invalidSettingMessages)}")
		{ }
	}
}
