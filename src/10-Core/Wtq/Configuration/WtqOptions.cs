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
	public int AnimationDurationMs { get; set; }
		= 250;

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="AnimationDurationMs"/> / 2.
	/// </summary>
	[JsonIgnore]
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
	public int AnimationTargetFps { get; set; }
		= 40;

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling on an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseOutQuart"/>.
	/// </summary>
	public AnimationType AnimationTypeToggleOn { get; set; }
		= AnimationType.EaseOutQuart;

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling off an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseInQuart"/>.
	/// </summary>
	public AnimationType AnimationTypeToggleOff { get; set; }
		= AnimationType.EaseInQuart;

	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	public ICollection<WtqAppOptions> Apps { get; set; }
		= [];

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.<br/>
	/// Defaults to "false".
	/// </summary>
	public bool AlwaysOnTop { get; set; }

	/// <inheritdoc cref="WtqAppOptions.AttachMode"/>
	public AttachMode AttachMode { get; set; }
		= AttachMode.FindOrStart;

	/// <summary>
	/// Whether the app should be toggled out when another app gets focus.<br/>
	/// Defaults to "true".
	/// </summary>
	public HideOnFocusLost HideOnFocusLost { get; set; }
		= HideOnFocusLost.Always;

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
	/// KWin-only.<br/>
	/// <br/>
	/// Whether the shortcut configuration in the KWin config panel should always follow WTQ's settings.<br/>
	/// <br/>
	/// When set to "true" (the default), WTQ is assumed to be authoritative on shortcuts, and will remove any
	/// shortcut configuration already present in the KWin config panel. This means that any changes made through
	/// the KWin config panel will quickly be overriden by WTQ.<br/>
	/// <br/>
	/// When set to "false", the only thing that WTQ will do, is keep shortcut slots around. I.e. if there are 2 hotkeys
	/// configured for app Y, WTQ will add 2 shortcut slots for said app in the KWin config panel. The actually
	/// configured keys are not touched, however. This makes the KWin config panel authoritative on what keys are
	/// configured for a shortcut.
	/// </summary>
	public bool HotkeyReset { get; set; }
		= true;

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.
	/// </summary>
	public ICollection<HotkeyOptions> Hotkeys { get; set; }
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
	public TaskbarIconVisibility TaskbarIconVisibility { get; set; }
		= TaskbarIconVisibility.AlwaysHidden;

	/// <summary>
	/// The default off-screen locations are kept separate, to prevent arrays from mergin during deserialization.
	/// We could do that by tweaking the JSON serializer, but that's way more complex.
	/// </summary>
	private static ICollection<OffScreenLocation> DefaultOffScreenLocations { get; } =
		[Above, Below, Left, Right];

	/// <summary>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.<br/>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br/>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </summary>
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

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

	public WtqAppOptions? GetAppOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return Apps.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	public WtqAppOptions GetAppOptionsByNameRequired(string name)
	{
		return GetAppOptionsByName(name) ?? throw new WtqException($"No options found for app with name '{name}'. These were found: {string.Join(", ", Apps.Select(a => a.Name))}.");
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

	public HideOnFocusLost GetHideOnFocusLostForApp(WtqAppOptions opts)
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

	public TaskbarIconVisibility GetTaskbarIconVisibilityForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.TaskbarIconVisibility ?? TaskbarIconVisibility;
	}

	public ICollection<OffScreenLocation> GetOffScreenLocationsForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.OffScreenLocations ?? OffScreenLocations ?? DefaultOffScreenLocations;
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

	public void PrepareForSave()
	{
		foreach (var app in Apps.ToList())
		{
			app.PrepareForSave();
		}

		foreach (var hk in Hotkeys.ToList())
		{
			if (hk.Modifiers == KeyModifiers.None || hk.Key == Keys.None)
			{
				Hotkeys.Remove(hk);
			}
		}
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