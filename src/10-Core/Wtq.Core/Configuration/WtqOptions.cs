namespace Wtq.Core.Configuration;

public sealed class WtqOptions
{
	[Required]
	public IEnumerable<WtqAppOptions> Apps { get; set; } = Enumerable.Empty<WtqAppOptions>();

	[Required]
	public IEnumerable<HotkeyOptions> Hotkeys { get; set; } = Enumerable.Empty<HotkeyOptions>();

	/// <summary>
	/// <para>If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
	/// <para>Zero based, eg. 0, 1, etc.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int MonitorIndex { get; set; }

	/// <summary>
	/// <para>What monitor to preferrably drop the terminal.</para>
	/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
	/// </summary>
	public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;
}