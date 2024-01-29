using Wtq.Core.Data;

namespace Wtq.Configuration;

public class WtqAppOptions
{
	// TODO: Use dict key?
	public string Name { get; set; }

	public IEnumerable<HotkeyOptions> Hotkeys { get; set; } = Array.Empty<HotkeyOptions>();

	[NotNull]
	[Required]
	public FindProcessOptions? FindExisting { get; set; }

	[NotNull]
	[Required]
	public CreateProcessOptions? StartNew { get; set; }

	public bool HasHotkey(WtqKeys key, WtqKeyModifiers modifiers)
	{
		return Hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString()
	{
		return FindExisting?.ProcessName ?? StartNew?.FileName ?? "<unknown>";
	}
}