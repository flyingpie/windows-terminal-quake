using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Models;

public class KWinWindow
{
	[JsonPropertyName("internalId")]
	public string? InternalId { get; set; }

	[JsonPropertyName("resourceClass")]
	public string? ResourceClass { get; set; }

	[JsonPropertyName("resourceName")]
	public string? ResourceName { get; set; }
}