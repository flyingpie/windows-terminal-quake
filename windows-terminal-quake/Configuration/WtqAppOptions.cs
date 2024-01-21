using System.Windows.Forms;
using Wtq.Native;

namespace Wtq.Configuration;

public class WtqAppOptions
{
	public IEnumerable<HotkeyOptions> Hotkeys { get; set; } = Array.Empty<HotkeyOptions>();

	[NotNull]
	[Required]
	public FindProcessOptions? FindExisting { get; set; }

	[NotNull]
	[Required]
	public CreateProcessOptions? StartNew { get; set; }

	public bool HasHotkey(Keys key, KeyModifiers modifiers)
	{
		return Hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString()
	{
		return FindExisting?.ProcessName ?? StartNew?.FileName ?? "<unknown>";
	}
}