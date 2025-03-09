using System.Linq.Expressions;
using System.Reflection;
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

	#region Animation



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

	[DefaultValue(false)]
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

	// public TValue GetValue<TValue>(WtqAppOptions app, Func<WtqSharedOptions, TValue> accessor)
	// {
	// 	var val = accessor(app);
	//
	// 	if (val != default)
	// 	{
	// 		return val;
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
		=> Guard.Against.Null(opts).AlwaysOnTop ?? AlwaysOnTop ?? GetDefaultValue<bool>(() => AlwaysOnTop);

	public AttachMode GetAttachModeForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.AttachMode ?? AttachMode ?? GetDefaultValue<AttachMode>(() => AttachMode);
	}

	public HideOnFocusLost GetHideOnFocusLostForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return Configuration.HideOnFocusLost.Always;
		// return opts.HideOnFocusLost ?? HideOnFocusLost;
	}

	public HorizontalAlign GetHorizontalAlignForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return Configuration.HorizontalAlign.Center;
		// return opts.HorizontalAlign ?? HorizontalAlign;
	}

	public float GetHorizontalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return 80;
		// return opts.HorizontalScreenCoverage ?? HorizontalScreenCoverage;
	}

	public int GetOpacityForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return 90;
		// return opts.Opacity ?? Opacity;
	}

	public TaskbarIconVisibility GetTaskbarIconVisibilityForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return Configuration.TaskbarIconVisibility.AlwaysHidden;
		// return opts.TaskbarIconVisibility ?? TaskbarIconVisibility;
	}

	public ICollection<OffScreenLocation> GetOffScreenLocationsForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.OffScreenLocations ?? OffScreenLocations ?? DefaultOffScreenLocations;
	}

	public float GetVerticalOffsetForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.VerticalOffset ?? VerticalOffset ?? 0; // TODO: Default
	}

	public float GetVerticalScreenCoverageForApp(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return opts.VerticalScreenCoverage ?? VerticalScreenCoverage ?? 0; // TODO: Default
	}

	public int GetMonitorIndex(WtqAppOptions opts)
	{
		return 0;
	}

	public AnimationType GetAnimationTypeToggleOn(WtqAppOptions opts)
	{
		return AnimationType.Linear;
	}

	public AnimationType GetAnimationTypeToggleOff(WtqAppOptions opts)
	{
		return AnimationType.Linear;
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