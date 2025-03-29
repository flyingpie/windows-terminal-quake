using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

/// <summary>
/// Defines WTQ-wide options, including the list of configured apps.
/// </summary>
[Display(Name = ":material-earth: Global options")]
public sealed class WtqOptions : WtqSharedOptions, IValidatableObject
{
	/// <summary>
	/// Path to wtq.schema.json.
	/// </summary>
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
	///			// ...
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
	///		"Hotkeys": [
	///			{ "Modifiers": "Control", "Key": "Q" }
	///		]
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.General)]
	[JsonPropertyOrder(102)]
	public ICollection<HotkeyOptions> Hotkeys { get; set; }
		= [];

	/// <summary>
	/// Whether to show the GUI when WTQ is started.
	/// </summary>
	[Display(GroupName = Gn.General, Name = "Show UI on start")]
	[DefaultValue(false)]
	[JsonPropertyOrder(103)]
	public bool? ShowUiOnStart { get; set; }

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.
	/// </summary>
	[Display(GroupName = Gn.Animation, Name = "Animation target FPS")]
	[DefaultValue(40)]
	[Range(5, 120)]
	public int? AnimationTargetFps { get; set; }

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

		foreach (var hk in Hotkeys.Where(hk => hk.IsEmpty).ToList()) // Explicit ToList() since we're modifying it from within the loop.
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