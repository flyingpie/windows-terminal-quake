using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
public sealed class WtqOptions
{
	/// <summary>
	/// Path to wtq.schema.json.
	/// </summary>
	[JsonPropertyName("$schema")]
	public string Schema { get; } = "wtq.schema.json";

	private int? _animationDurationMsSwitchingApps;

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	public int AnimationDurationMs { get; init; }
		= 250;

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="AnimationDurationMs"/> / 2.
	/// </summary>
	public int AnimationDurationMsWhenSwitchingApps
	{
		get => _animationDurationMsSwitchingApps
				?? (int)Math.Round(AnimationDurationMs * .5f);
		set => _animationDurationMsSwitchingApps = value;
	}

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br/>
	/// Defaults to 40.
	/// </summary>
	public int AnimationTargetFps { get; init; }
		= 40;

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling on an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseOutQuart"/>.
	/// </summary>
	public AnimationType AnimationTypeToggleOn { get; init; }
		= AnimationType.EaseOutQuart;

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling off an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseInQuart"/>.
	/// </summary>
	public AnimationType AnimationTypeToggleOff { get; init; }
		= AnimationType.EaseInQuart;

	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	[Required]
	public IEnumerable<WtqAppOptions> Apps { get; init; }
		= [];

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.<br/>
	/// Defaults to "false".
	/// </summary>
	public bool AlwaysOnTop { get; init; }

	/// <inheritdoc cref="WtqAppOptions.AttachMode"/>
	public AttachMode AttachMode { get; init; }
		= AttachMode.FindOrStart;

	/// <summary>
	/// Whether the app should be toggled out when another app gets focus.<br/>
	/// Defaults to "true".
	/// </summary>
	public bool HideOnFocusLost { get; init; }
		= true;

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.<br/>
	/// Defaults to <see cref="HorizontalAlign.Center"/>.
	/// </summary>
	public HorizontalAlign HorizontalAlign { get; init; }
		= HorizontalAlign.Center;

	/// <summary>
	/// Horizontal screen coverage, as a percentage.<br/>
	/// Defaults to "100".
	/// </summary>
	public float HorizontalScreenCoverage { get; init; }
		= 95f;

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.
	/// </summary>
	[Required]
	public IEnumerable<HotkeyOptions> Hotkeys { get; init; }
		= [];

	/// <summary>
	/// If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.<br/>
	/// Zero based, eg. 0, 1, etc.<br/>
	/// Defaults to "0".
	/// </summary>
	public int MonitorIndex { get; init; }

	/// <summary>
	/// Make the window see-through (applies to the entire window, including the title bar).<br/>
	/// 0 (invisible) - 100 (opaque)..<br/>
	/// Defaults to "100".
	/// </summary>
	public int Opacity { get; init; }
		= 100;

	/// <summary>
	/// What monitor to preferrably drop the app.<br/>
	/// "WithCursor" (default), "Primary" or "AtIndex".
	/// </summary>
	public PreferMonitor PreferMonitor { get; init; }
		= PreferMonitor.WithCursor;

	/// <summary>
	/// When to show the terminal window icon on the taskbar.<br/>
	/// "AlwaysHidden", "AlwaysVisible" or "WhenTerminalVisible".<br/>
	/// Defaults to "AlwaysHidden".
	/// </summary>
	public TaskBarIconVisibility TaskBarIconVisibility { get; init; }
		= TaskBarIconVisibility.AlwaysHidden;

	/// <summary>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.<br/>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br/>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </summary>
	public IEnumerable<OffScreenLocation> OffScreenLocations { get; init; }
		= [Above, Below, Left, Right];

	/// <summary>
	/// How much room to leave between the top of the terminal and the top of the screen, in pixels.<br/>
	/// Defaults to "0".
	/// </summary>
	public int VerticalOffset { get; init; }

	/// <summary>
	/// Vertical screen coverage as a percentage (0-100).<br/>
	/// Defaults to "100".
	/// </summary>
	public float VerticalScreenCoverage { get; init; }
		= 95f;

	public WtqAppOptions? GetAppOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return Apps.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	public WtqAppOptions GetAppOptionsByNameRequired(string name)
	{
		return GetAppOptionsByName(name) ?? throw new WtqException($"No options found for app with name '{name}'.");
	}

	public bool GetAlwaysOnTopForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.AlwaysOnTop ?? AlwaysOnTop;
	}

	public AttachMode GetAttachModeForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.AttachMode ?? AttachMode;
	}

	public bool GetHideOnFocusLostForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.HideOnFocusLost ?? HideOnFocusLost;
	}

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

	public IEnumerable<OffScreenLocation> GetOffScreenLocationsForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.OffScreenLocations ?? OffScreenLocations;
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