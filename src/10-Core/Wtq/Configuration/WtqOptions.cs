using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
[Display(Name = ":material-earth: Global options")]
public sealed class WtqOptions : WtqSharedOptions
{
	/// <summary>
	/// Path to wtq.schema.json.<br/>
	/// Used for adding intellisense-like features to editors that support JSON schema (such as VSCode).
	/// </summary>
	/// <remarks>
	/// Fixed value, changes to this property in the wtq.jsonc will be ignored/overwritten.
	/// </remarks>
	[DisplayFlags(IsVisible = false)]
	[JsonPropertyName("$schema")]
	[JsonPropertyOrder(0)]
	public string Schema { get; } = "wtq.schema.json";

	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	/// <remarks>
	/// See the GUI and the <a href="#app-examples">docs</a> for some examples.
	/// </remarks>
	/// <example>
	/// <code>
	/// {
	/// 	"Apps": [
	/// 		{ "Name": "App 1" },
	/// 		{ "Name": "App 2" },
	/// 		// ...
	/// 	]
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.General)]
	[JsonPropertyOrder(101)]
	[Required]
	public ICollection<WtqAppOptions> Apps { get; set; }
		= [];

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.<br/>
	/// Optional.
	/// </summary>
	/// <example>
	/// <code>
	/// {
	/// 	"Hotkeys": [
	/// 		{ "Modifiers": "Control", "Key": "Q" }
	/// 	]
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.General)]
	[JsonPropertyOrder(102)]
	public ICollection<HotkeyOptions> Hotkeys { get; set; }
		= [];


	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.
	/// </summary>
	[DefaultValue(40)]
	[Display(GroupName = Gn.Animation, Name = "Animation target FPS")]
	[JsonPropertyOrder(103)]
	[Range(5, 120)]
	public int? AnimationTargetFps { get; set; }

	/// <summary>
	/// Options related to the HTTP API, that can be used to control WTQ programmatically.
	/// </summary>
	public WtqApiOptions? Api { get; set; }

	/// <summary>
	/// Sometimes functionality is added or changed that carries more risk of introducing bugs.<br/>
	/// <br/>
	/// For these cases, such functionality can be put behind a "feature flag", which makes them opt-in or opt-out.<br/>
	/// That way, we can still merge to master, and make it part of the stable release version (reducing branches and dev builds and what not),
	/// but still have a way back should things go awry.
	/// </summary>
	[Display(GroupName = Gn.General, Name = "Feature flags")]
	[JsonPropertyOrder(104)]
	public FeatureFlags? FeatureFlags { get; set; }

	/// <summary>
	/// Whether to show the GUI when WTQ is started.
	/// </summary>
	[DefaultValue(false)]
	[Display(GroupName = Gn.General, Name = "Show UI on start")]
	[JsonPropertyOrder(105)]
	public bool? ShowUiOnStart { get; set; }

	/// <summary>
	/// Called right after the options are loaded from file.
	/// </summary>
	public void OnPostConfigure()
	{
		foreach (var app in Apps ?? [])
		{
			app.OnPostConfigure(this);
		}
	}

	/// <summary>
	/// Called right before the options are saved to file.
	/// </summary>
	public void PrepareForSave()
	{
		foreach (var app in Apps.ToList())
		{
			app.PrepareForSave();
		}

		// Explicit ToList() since we're modifying it from within the loop.
		foreach (var hk in Hotkeys.Where(hk => hk.Sequence.IsEmpty).ToList())
		{
			Hotkeys.Remove(hk);
		}
	}

	protected override IEnumerable<ValidationResult> OnValidate(ValidationContext context)
	{
		// TODO
		return [];
	}
}