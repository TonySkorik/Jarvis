using System;
using System.Collections.Generic;
using System.Text;

namespace Jarvis.Core.Configuration
{
	public interface IDatabaseSettingsProvider
	{
		public string GetConnectionString();
	}
}
