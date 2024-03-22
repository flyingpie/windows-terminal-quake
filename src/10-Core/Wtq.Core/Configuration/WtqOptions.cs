namespace Wtq.Core.Configuration;

public sealed class WtqOptions
{
	[Required]
	public IEnumerable<WtqAppOptions> Apps { get; set; } = [];

	public AttachMode AttachMode { get; set; } = AttachMode.FindOrStart;

	[Required]
	public IEnumerable<HotKeyOptions> HotKeys { get; set; } = [];

	/// <summary>
	/// <para>Gets or sets if "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
	/// <para>Zero based, eg. 0, 1, etc.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int MonitorIndex { get; set; }

	/// <summary>
	/// <para>Gets or sets what monitor to preferrably drop the terminal.</para>
	/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
	/// </summary>
	public PreferMonitor PreferMonitor { get; set; } = PreferMonitor.WithCursor;
}