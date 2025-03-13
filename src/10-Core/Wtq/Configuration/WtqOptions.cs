using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
public sealed class WtqOptions : WtqSharedOptions, IValidatableObject
{
	/// <summary>
	/// The default off-screen locations are kept separate, to prevent arrays from mergin during deserialization.
	/// We could do that by tweaking the JSON serializer, but that's way more complex.
	/// </summary>
	private static ICollection<OffScreenLocation> DefaultOffScreenLocations { get; } =
		[Above, Below, Left, Right];

	/// <summary>
	/// Path to wtq.schema.json.
	/// </summary>
	[JsonPropertyName("$schema")]
	public string Schema { get; } = "wtq.schema.json";

	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	public ICollection<WtqAppOptions> Apps { get; set; }
		= [];

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.
	/// </summary>
	public ICollection<HotkeyOptions> Hotkeys { get; set; }
		= [];

	[Display(Name = "Show UI on start")]
	[DefaultValue(false)]
	public bool? ShowUiOnStart { get; set; }

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br/>
	/// Defaults to 40.
	/// </summary>
	[Display(Name = "Animation target FPS")]
	[DefaultValue(40)]
	public int? AnimationTargetFps { get; set; }

	public WtqAppOptions? GetAppOptionsByName(string name)
	{
		Guard.Against.NullOrWhiteSpace(name);

		return Apps.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	public WtqAppOptions GetAppOptionsByNameRequired(string name)
		=> GetAppOptionsByName(name)
			?? throw new WtqException($"No options found for app with name '{name}'. These were found: {string.Join(", ", Apps.Select(a => a.Name))}.");

	public bool GetAlwaysOnTopForApp(WtqAppOptions opts)
		=> GetCascadingValue<bool>(o => o.AlwaysOnTop, opts);

	public AttachMode GetAttachModeForApp(WtqAppOptions opts)
		=> GetCascadingValue<AttachMode>(o => o.AttachMode, opts);

	public HideOnFocusLost GetHideOnFocusLostForApp(WtqAppOptions opts)
		=> GetCascadingValue<HideOnFocusLost>(o => o.AttachMode, opts);

	public HorizontalAlign GetHorizontalAlignForApp(WtqAppOptions opts)
		=> GetCascadingValue<HorizontalAlign>(o => o.HorizontalAlign, opts);

	public float GetHorizontalScreenCoverageForApp(WtqAppOptions opts)
		=> GetCascadingValue<float>(o => o.HorizontalScreenCoverage, opts);

	/// <summary>
	/// <see cref="WtqSharedOptions.HorizontalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public float HorizontalScreenCoverageIndexForApp(WtqAppOptions opts)
		=> GetHorizontalScreenCoverageForApp(opts) / 100f;

	public int GetOpacityForApp(WtqAppOptions opts)
		=> GetCascadingValue<int>(o => o.Opacity, opts);

	public TaskbarIconVisibility GetTaskbarIconVisibilityForApp(WtqAppOptions opts)
		=> GetCascadingValue<TaskbarIconVisibility>(o => o.TaskbarIconVisibility, opts);

	public ICollection<OffScreenLocation> GetOffScreenLocationsForApp(WtqAppOptions opts)
		=> GetCascadingValue<ICollection<OffScreenLocation>>(o => o.OffScreenLocations, opts) ?? DefaultOffScreenLocations;

	public float GetVerticalOffsetForApp(WtqAppOptions opts)
		=> GetCascadingValue<float>(o => o.VerticalOffset, opts);

	public float GetVerticalScreenCoverageForApp(WtqAppOptions opts)
		=> GetCascadingValue<float>(o => o.VerticalScreenCoverage, opts);

	/// <summary>
	/// <see cref="WtqSharedOptions.VerticalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public float VerticalScreenCoverageIndexForApp(WtqAppOptions opts)
		=> GetVerticalScreenCoverageForApp(opts) / 100f;

	public int GetMonitorIndex(WtqAppOptions opts)
		=> GetCascadingValue<int>(o => o.MonitorIndex, opts);

	#region Animation

	public int GetAnimationDurationMs(WtqAppOptions opts)
		=> GetCascadingValue<int>(o => o.AnimationDurationMs, opts);

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="WtqSharedOptions.AnimationDurationMs"/> / 2.
	/// </summary>
	public int GetAnimationDurationMsWhenSwitchingApps(WtqAppOptions opts)
		=> (int)Math.Round(GetAnimationDurationMs(opts) * .5f);

	public int GetAnimationTargetFps()
		=> AnimationTargetFps ?? DefaultValue.For<int>(() => AnimationTargetFps);

	public AnimationType GetAnimationTypeToggleOn(WtqAppOptions opts)
		=> GetCascadingValue<AnimationType>(o => o.AnimationTypeToggleOn, opts);

	public AnimationType GetAnimationTypeToggleOff(WtqAppOptions opts)
		=> GetCascadingValue<AnimationType>(o => o.AnimationTypeToggleOff, opts);

	#endregion

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

	#region Validation

	[JsonIgnore]
	public IEnumerable<ValidationResult> ValidationResults => this.Validate();

	public IEnumerable<ValidationResult> Validate(ValidationContext context)
	{
		yield return new ValidationResult("Sup");
	}

	#endregion

	private TValue GetCascadingValue<TValue>(
		Expression<Func<WtqSharedOptions, object?>> expr,
		WtqAppOptions app)
	{
		Guard.Against.Null(expr);
		Guard.Against.Null(app);

		var v = expr.Compile();

		var fromApp = v(app);
		if (fromApp != null)
		{
			return (TValue)fromApp;
		}

		var fromGlb = v(this);
		if (fromGlb != null)
		{
			return (TValue)fromGlb;
		}

		return DefaultValue.For<TValue>(expr);
	}
}