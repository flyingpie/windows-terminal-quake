namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
public sealed class WtqAppOptions : WtqSharedOptions, IValidatableObject
{
	private ICollection<ProcessArgument> _argumentList = [];
	private ICollection<HotkeyOptions> _hotkeys = [];
	private string? _processName;

	#region App

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads.<br/>
	/// Appears in logs.
	/// </summary>
	[JsonPropertyOrder(1)]
	[NotNull]
	[Required]
	public string? Name { get; set; }

	[JsonPropertyOrder(2)]
	public ICollection<HotkeyOptions> Hotkeys
	{
		get => _hotkeys;
		set => _hotkeys = value ?? [];
	}

	#endregion

	#region Process

	/// <summary>
	/// The <strong>filename</strong> to use when starting a new process for the app.<br/>
	/// E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.
	/// </summary>
	[DisplayName("Filename")]
	[JsonPropertyOrder(10)]
	[NotNull]
	[Required]
	public string? FileName { get; set; }

	/// <summary>
	/// Apps sometimes have <Emph>process names</Emph> different from their <Emph>filenames</Emph>.
	/// This field can be used to look for the process name in such cases. Windows Terminal is an
	/// example, with filename <Emph>wt</Emph>, and process name <Emph>WindowsTerminal</Emph>.
	/// </summary>
	// [DisplayName("Process name")]
	[JsonPropertyOrder(11)]
	public string? ProcessName
	{
		get => _processName;
		set => _processName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <see cref="AttachMode"/> that starts the app.
	/// </summary>
	[JsonPropertyOrder(12)]
	public string? Arguments { get; set; }

	[JsonPropertyOrder(13)]
	public ICollection<ProcessArgument> ArgumentsList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	// /// <inheritdoc cref="WtqOptions.AttachMode"/>
	// [JsonPropertyOrder(14)]
	// public AttachMode? AttachMode { get; set; }

	[JsonPropertyOrder(15)]
	public string? WindowTitle { get; set; }

	#endregion

	#region Behavior

	/// <inheritdoc cref="WtqOptions.AlwaysOnTop"/>
	[JsonPropertyOrder(99)]
	public bool? AlwaysOnTop { get; set; }

	/// <inheritdoc cref="WtqOptions.HideOnFocusLost"/>
	[JsonPropertyOrder(99)]
	public HideOnFocusLost? HideOnFocusLost { get; set; }

	/// <inheritdoc cref="WtqOptions.Opacity"/>
	[JsonPropertyOrder(99)]
	public int? Opacity { get; set; }

	/// <inheritdoc cref="WtqOptions.TaskbarIconVisibility"/>
	[JsonPropertyOrder(99)]
	public TaskbarIconVisibility? TaskbarIconVisibility { get; set; }

	#endregion

	#region Position

	/// <inheritdoc cref="WtqOptions.HorizontalAlign"/>
	[JsonPropertyOrder(99)]
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <inheritdoc cref="WtqOptions.HorizontalScreenCoverage"/>
	[JsonPropertyOrder(99)]
	public float? HorizontalScreenCoverage { get; set; }

	/// <inheritdoc cref="WtqOptions.OffScreenLocations"/>
	[JsonPropertyOrder(99)]
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalOffset"/>
	[JsonPropertyOrder(99)]
	public int? VerticalOffset { get; set; }

	/// <inheritdoc cref="WtqOptions.VerticalScreenCoverage"/>
	[JsonPropertyOrder(99)]
	public float? VerticalScreenCoverage { get; set; }

	#endregion

	#region Monitor

	/// <inheritdoc cref="WtqOptions.MonitorIndex"/>
	[JsonPropertyOrder(99)]
	public int? MonitorIndex { get; set; }

	/// <inheritdoc cref="WtqOptions.PreferMonitor"/>
	[JsonPropertyOrder(99)]
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// Attempt to set the window title to a specific value.
	/// </summary>
	[JsonPropertyOrder(99)]
	public string? WindowTitleOverride { get; set; }

	#endregion

	#region Animation

	/// <inheritdoc cref="WtqOptions.AnimationDurationMs"/>
	[JsonPropertyOrder(50)]
	public int? AnimationDurationMs { get; set; }

	/// <inheritdoc cref="WtqOptions.AnimationDurationMsWhenSwitchingApps"/>
	[JsonIgnore]
	public int? AnimationDurationMsWhenSwitchingApps { get; set; }

	/// <inheritdoc cref="WtqOptions.AnimationTargetFps"/>
	[JsonPropertyOrder(51)]
	public int? AnimationTargetFps { get; set; }

	#endregion

	#region Validation

	[JsonIgnore]
	public bool IsValid => true; //!this.Validate().Any();

	[JsonIgnore]
	public IEnumerable<ValidationResult> ValidationResults => this.Validate();

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrWhiteSpace(Name))
		{
			yield return new("A name is required.", [nameof(Name)]);
		}

		if (string.IsNullOrWhiteSpace(FileName) && string.IsNullOrWhiteSpace(ProcessName) && string.IsNullOrWhiteSpace(WindowTitle))
		{
			yield return new("Either a <strong>filename</strong>, a <strong>process name</strong> or a <strong>window title</strong> needs to be set.", [nameof(FileName), nameof(ProcessName), nameof(WindowTitle)]);
		}

		if (Hotkeys.Count == 0)
		{
			yield return new("Specify at least 1 hotkey.", [nameof(Hotkeys)]);
		}
	}

	#endregion

	public bool HasHotkey(Keys key, KeyModifiers modifiers)
	{
		return Hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	/// <summary>
	/// Called before the settings are persisted to a file.
	/// </summary>
	public void PrepareForSave()
	{
		foreach (var hk in Hotkeys.ToList())
		{
			if (hk.IsEmpty)
			{
				Hotkeys.Remove(hk);
			}
		}
	}

	public override string ToString() => Name;
}