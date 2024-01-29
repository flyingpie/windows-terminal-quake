namespace Wtq.Configuration;

public sealed class WtqOptions
{
	[Required]
	public IEnumerable<WtqAppOptions> Apps { get; set; } = Enumerable.Empty<WtqAppOptions>();

	[Required]
	public IEnumerable<HotkeyOptions> Hotkeys { get; set; } = Enumerable.Empty<HotkeyOptions>();
}