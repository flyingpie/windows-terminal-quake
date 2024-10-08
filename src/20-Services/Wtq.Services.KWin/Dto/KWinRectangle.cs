using System.Text.Json.Serialization;

namespace Wtq.Services.KWin.Dto;

public sealed class KWinRectangle
{
	[JsonPropertyName("x")]
	public int? X { get; set; }

	[JsonPropertyName("y")]
	public int? Y { get; set; }

	[JsonPropertyName("width")]
	public int? Width { get; set; }

	[JsonPropertyName("height")]
	public int? Height { get; set; }

	[JsonPropertyName("top")]
	public int? Top { get; set; }

	[JsonPropertyName("bottom")]
	public int? Bottom { get; set; }

	[JsonPropertyName("left")]
	public int? Left { get; set; }

	[JsonPropertyName("right")]
	public int? Right { get; set; }

	public Point ToPoint() => new(X ?? -1, Y ?? -1);

	public Size ToSize() => new(Width ?? -1, Height ?? -1);

	public Rectangle ToRect() => new(X ?? -1, Y ?? -1, Width ?? -1, Height ?? -1);

	public override string ToString() => $"(X:{X}, Y:{Y}, Width:{Width}, Height:{Height}) (Top:{Top} Bottom:{Bottom} Left:{Left} Right:{Right})";
}