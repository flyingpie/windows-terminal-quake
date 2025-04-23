namespace Wtq.Services.KWin.Dto;

public class KWinWindow
{
	[JsonPropertyName("desktopFileName")]
	public string? DesktopFileName { get; set; }

	[JsonPropertyName("caption")]
	public string? Caption { get; set; }

	[JsonPropertyName("frameGeometry")]
	public KWinRectangle? FrameGeometry { get; set; }

	[JsonPropertyName("hidden")]
	public bool Hidden { get; set; }

	[JsonPropertyName("internalId")]
	public string? InternalId { get; set; }

	[JsonPropertyName("keepAbove")]
	public bool KeepAbove { get; set; }

	[JsonPropertyName("layer")]
	public int Layer { get; set; }

	[JsonPropertyName("minimized")]
	public bool Minimized { get; set; }

	[JsonPropertyName("resourceClass")]
	public string? ResourceClass { get; set; }

	[JsonPropertyName("resourceName")]
	public string? ResourceName { get; set; }

	[JsonPropertyName("skipPager")]
	public bool SkipPager { get; set; }

	[JsonPropertyName("skipSwitcher")]
	public bool SkipSwitcher { get; set; }

	[JsonPropertyName("skipTaskbar")]
	public bool SkipTaskbar { get; set; }
}