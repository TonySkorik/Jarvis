using Newtonsoft.Json;

namespace Jarvis.SstCloud.Core.Model;

public abstract class IdentifiableValue
{
	[JsonProperty("id")]
	public int Id { get; set; }
}