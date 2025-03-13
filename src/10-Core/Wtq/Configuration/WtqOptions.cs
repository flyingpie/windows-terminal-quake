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
	public int? AnimationTargetFps { get; set; }

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

	public void OnPostConfigure()
	{
		foreach (var app in Apps ?? [])
		{
			app.OnPostConfigure(this);
		}
	}

	[JsonIgnore]
	public IEnumerable<ValidationResult> ValidationResults => this.Validate();

	public IEnumerable<ValidationResult> Validate(ValidationContext context)
	{
		yield return new ValidationResult("Sup");
	}
}