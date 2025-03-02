using System.Reflection;
using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
public sealed class WtqOptions : WtqSharedOptions, IValidatableObject
{
	/// <summary>
	/// Path to wtq.schema.json.
	/// </summary>
	[JsonPropertyName("$schema")]
	public string Schema { get; } = "wtq.schema.json";

	/// <summary>
	/// The default off-screen locations are kept separate, to prevent arrays from mergin during deserialization.
	/// We could do that by tweaking the JSON serializer, but that's way more complex.
	/// </summary>
	private static ICollection<OffScreenLocation> DefaultOffScreenLocations { get; } =
		[Above, Below, Left, Right];

	#region Animation

	private int? _animationDurationMsSwitchingApps;

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	[DefaultValue(250)]
	public int? AnimationDurationMs { get; set; }

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br/>
	/// Defaults to 40.
	/// </summary>
	[DefaultValue(40)]
	public int? AnimationTargetFps { get; set; }

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="AnimationDurationMs"/> / 2.
	/// </summary>
	[JsonIgnore]
	public int AnimationDurationMsWhenSwitchingApps
	{
		get => 125; //_animationDurationMsSwitchingApps
			// ?? (int)Math.Round(AnimationDurationMs * .5f);
		set => _animationDurationMsSwitchingApps = value;
	}

	#endregion

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

	public bool ShowUiOnStart { get; set; }

	public int GetAnimationTargetFps()
	{
		if (AnimationTargetFps != null)
		{
			return AnimationTargetFps.Value;
		}

		var x = (int)SystemExtensions.GetMemberInfo(() => AnimationDurationMs).GetCustomAttribute<DefaultValueAttribute>().Value;

		return x;
	}

	// public TValue GetValue<TValue>(Func<WtqOptions, TValue> accessor)
	// {
	// 	var val
	//
	// 	if (AnimationTargetFps != null)
	// 	{
	// 		return AnimationTargetFps.Value;
	// 	}
	//
	// 	var x = (int)SystemExtensions.GetMemberInfo(() => AnimationDurationMs).GetCustomAttribute<DefaultValueAttribute>().Value;
	//
	// 	return x;
	// }

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

		return Configuration.AttachMode.FindOrStart;
		// return opts.AttachMode ?? AttachMode;
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

		foreach (var p in GetType().GetProperties())
		{
			var attr = p.GetCustomAttribute<DefaultValueAttribute>();
			if (attr == null)
			{
				continue;
			}

			var val = p.GetValue(this);

			if (val != null && val.Equals(attr.Value))
			{
				Console.WriteLine($"Clearing default '{p.Name}'");
				p.SetValue(this, default);
			}
		}

	}


	[JsonIgnore]
	public IEnumerable<ValidationResult> ValidationResults => this.Validate();

	public IEnumerable<ValidationResult> Validate(ValidationContext context)
	{
		yield return new ValidationResult("Sup");
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