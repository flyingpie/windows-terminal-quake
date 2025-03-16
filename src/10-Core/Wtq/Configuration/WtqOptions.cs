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
	[JsonPropertyOrder(0)]
	public string Schema { get; } = "wtq.schema.json";

	/// <summary>
	/// Applications to enable Quake-style dropdown for.
	/// </summary>
	[JsonPropertyOrder(101)]
	public ICollection<WtqAppOptions> Apps { get; set; }
		= [];

	/// <summary>
	/// Global hotkeys, that toggle either the first, or the most recently toggled app.
	/// </summary>
	[JsonPropertyOrder(102)]
	public ICollection<HotkeyOptions> Hotkeys { get; set; }
		= [];

	/// <summary>
	/// Whether to show the GUI when WTQ is started.
	/// </summary>
	[Display(Name = "Show UI on start")]
	[DefaultValue(false)]
	[JsonPropertyOrder(103)]
	public bool? ShowUiOnStart { get; set; }

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br/>
	/// Defaults to 40.
	/// </summary>
	[Display(Name = "Animation target FPS")]
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