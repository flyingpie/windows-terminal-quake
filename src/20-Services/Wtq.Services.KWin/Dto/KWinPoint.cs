using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Dto;

public class KWinPoint
{
	[JsonPropertyName("x")]
	public int? X { get; set; }

	[JsonPropertyName("y")]
	public int? Y { get; set; }

	public Point ToPoint() => new Point(X.Value, Y.Value);
}