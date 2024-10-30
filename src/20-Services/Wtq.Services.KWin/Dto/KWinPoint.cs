namespace Wtq.Services.KWin.Dto;

public sealed class KWinPoint
{
	[JsonPropertyName("x")]
	public int? X { get; set; }

	[JsonPropertyName("y")]
	public int? Y { get; set; }

	public Point ToPoint() => new(X ?? 0, Y ?? 0);
}