using Newtonsoft.Json;

namespace SstCloud.Core.Model;

public abstract class IdentifiableValue
{
	[JsonProperty("id")]
	public int Id { get; set; }
}