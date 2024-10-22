using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Dto;

public class KWinGetWindowListResponse
{
	[JsonPropertyName("windows")]
	public ICollection<KWinWindow> Windows { get; set; }
}