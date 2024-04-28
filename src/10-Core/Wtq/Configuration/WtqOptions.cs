namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
public sealed class WtqOptions
{
	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	[Required]
	public IEnumerable<WtqAppOptions> Apps { get; set; }
		= [];

	/// <inheritdoc cref="WtqAppOptions.AttachMode"/>
	public AttachMode AttachMode { get; set; }
		= AttachMode.FindOrStart;

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.<br/>
	/// Defaults to <see cref="HorizontalAlign.Center"/>.
	/// </summary>
	public HorizontalAlign HorizontalAlign { get; set; }
		= HorizontalAlign.Center;

	/// <summary>
	/// Horizontal screen coverage, as a percentage.<br/>
	/// Defaults to "100".
	/// </summary>
	public float HorizontalScreenCoverage { get; set; }
		= 95f;

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.
	/// </summary>
	[Required]
	public IEnumerable<HotKeyOptions> HotKeys { get; set; }
		= [];

	/// <summary>
	/// If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.<br/>
	/// Zero based, eg. 0, 1, etc.<br/>
	/// Defaults to "0".
	/// </summary>
	public int MonitorIndex { get; set; }

	/// <summary>
	/// Make the window see-through (applies to the entire window, including the title bar).<br/>
	/// 0 (invisible) - 100 (opaque)..<br/>
	/// Defaults to "100".
	/// </summary>
	public int Opacity { get; set; }
		= 100;

	/// <summary>
	/// What monitor to preferrably drop the app.<br/>
	/// "WithCursor" (default), "Primary" or "AtIndex".
	/// </summary>
	public PreferMonitor PreferMonitor { get; set; }
		= PreferMonitor.WithCursor;

	/// <summary>
	/// When to show the terminal window icon on the taskbar.<br/>
	/// "AlwaysHidden", "AlwaysVisible" or "WhenTerminalVisible".<br/>
	/// Defaults to "AlwaysHidden".
	/// </summary>
	public TaskBarIconVisibility TaskBarIconVisibility { get; set; }
		= TaskBarIconVisibility.AlwaysHidden;

	/// <summary>
	/// How much room to leave between the top of the terminal and the top of the screen, in pixels.<br/>
	/// Defaults to "0".
	/// </summary>
	public int VerticalOffset { get; set; }

	/// <summary>
	/// Vertical screen coverage as a percentage (0-100).<br/>
	/// Defaults to "100".
	/// </summary>
	public float VerticalScreenCoverage { get; set; }
		= 95f;

	public HorizontalAlign GetHorizontalAlignForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.HorizontalAlign ?? HorizontalAlign;
	}

	public float GetHorizontalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.HorizontalScreenCoverage ?? HorizontalScreenCoverage;
	}

	public int GetOpacityForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.Opacity ?? Opacity;
	}

	public TaskBarIconVisibility GetTaskbarIconVisibilityForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.TaskbarIconVisibility ?? TaskBarIconVisibility;
	}

	public float GetVerticalOffsetForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.VerticalOffset ?? VerticalOffset;
	}

	public float GetVerticalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.VerticalScreenCoverage ?? VerticalScreenCoverage;
	}

	/// <summary>
	/// <see cref="HorizontalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	internal float HorizontalScreenCoverageIndexForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return GetHorizontalScreenCoverageForApp(opts) / 100f;
	}

	/// <summary>
	/// <see cref="VerticalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	internal float VerticalScreenCoverageIndexForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return GetVerticalScreenCoverageForApp(opts) / 100f;
	}
}