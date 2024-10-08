namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
public sealed class WtqAppOptions
{
	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <see cref="AttachMode"/> that starts the app.
	/// </summary>
	public string? Arguments { get; set; }

	/// <inheritdoc cref="WtqOptions.AlwaysOnTop"/>
	public bool? AlwaysOnTop { get; init; }

	/// <inheritdoc cref="WtqOptions.AttachMode"/>
	public AttachMode? AttachMode { get; set; }

	/// <summary>
	/// Path to an executable that should be run when we're starting an app instance ourselves.
	/// </summary>
	[NotNull]
	[Required]
	public string? FileName { get; set; }

	/// <inheritdoc cref="WtqOptions.HideOnFocusLost"/>
	public bool? HideOnFocusLost { get; init; }

	/// <inheritdoc cref="WtqOptions.HorizontalAlign"/>
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <inheritdoc cref="WtqOptions.HorizontalScreenCoverage"/>
	public float? HorizontalScreenCoverage { get; set; }

	public IEnumerable<HotKeyOptions> HotKeys { get; set; } = [];

	/// <inheritdoc cref="WtqOptions.MonitorIndex"/>
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads.<br/>
	/// Appears in logs.
	/// </summary>
	[NotNull]
	[Required]
	public string? Name { get; set; }

	/// <inheritdoc cref="WtqOptions.Opacity"/>
	public int? Opacity { get; set; }

	/// <inheritdoc cref="WtqOptions.PreferMonitor"/>
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// The name of the process to look for, when searching for an existing app instance.
	/// </summary>
	public string? ProcessName { get; set; }

	/// <inheritdoc cref="WtqOptions.TaskBarIconVisibility"/>
	public TaskBarIconVisibility? TaskbarIconVisibility { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalOffset"/>
	public int? VerticalOffset { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalScreenCoverage"/>
	public float? VerticalScreenCoverage { get; set; }

	public bool HasHotKey(Keys key, KeyModifiers modifiers)
	{
		return HotKeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString() => Name;
}