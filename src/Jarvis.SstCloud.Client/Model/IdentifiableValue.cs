using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Jarvis.SstCloud.Client.Model
{
	public abstract class IdentifiableValue
	{
		[JsonProperty("id")]
		public int Id { get; set; }
	}
}
