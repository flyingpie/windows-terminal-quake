using Wtq.Core.Data;

namespace Wtq.Core.Configuration;

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

	/// <summary>
	/// <para>If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
	/// <para>Zero based, eg. 0, 1, etc.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// <para>What monitor to preferrably drop the terminal.</para>
	/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
	/// </summary>
	public PreferMonitor? PreferMonitor { get; set; }

	public bool HasHotkey(WtqKeys key, WtqKeyModifiers modifiers)
	{
		return Hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString()
	{
		return FindExisting?.ProcessName ?? StartNew?.FileName ?? "<unknown>";
	}
}