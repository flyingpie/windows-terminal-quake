namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
public sealed class WtqAppOptions : WtqSharedOptions, IValidatableObject
{
	private ICollection<ProcessArgument> _argumentList = [];
	private ICollection<HotkeyOptions> _hotkeys = [];
	private string? _fileName;
	private string? _processName;
	private string? _windowTitle;
	private string? _windowTitleOverride;

	public WtqOptions Global { get; set; } = null!;

	#region 1000 - App

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads.<br/>
	/// Appears in logs.
	/// </summary>
	[NotNull]
	[Required]
	[JsonPropertyOrder(1001)]
	public string? Name { get; set; }

	/// <summary>
	/// One or more keyboard shortcuts that toggle in- and out this particular app.
	/// </summary>
	[JsonPropertyOrder(1002)]
	public ICollection<HotkeyOptions> Hotkeys
	{
		get => _hotkeys;
		set => _hotkeys = value ?? [];
	}

	#endregion

	#region 2000 - Process

	/// <summary>
	/// The <strong>filename</strong> to use when starting a new process for the app.<br/>
	/// E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.
	/// </summary>
	[Display(Name = "Filename")]
	[JsonPropertyOrder(2001)]
	[Required]
	public string? FileName
	{
		get => _fileName;
		set => _fileName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Apps sometimes have <Emph>process names</Emph> different from their <Emph>filenames</Emph>.
	/// This field can be used to look for the process name in such cases. Windows Terminal is an
	/// example, with filename <Emph>wt</Emph>, and process name <Emph>WindowsTerminal</Emph>.
	/// </summary>
	[Display(Name = "Process name")]
	[JsonPropertyOrder(2003)]
	public string? ProcessName
	{
		get => _processName;
		set => _processName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <see cref="AttachMode"/> that starts the app.
	/// </summary>
	[JsonPropertyOrder(2004)]
	public string? Arguments { get; set; }

	[Display(Name = "Argument list")]
	[JsonPropertyOrder(2004)]
	public ICollection<ProcessArgument> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	[Display(Name = "Window title")]
	[JsonPropertyOrder(2006)]
	public string? WindowTitle
	{
		get => _windowTitle;
		set => _windowTitle = value?.EmptyOrWhiteSpaceToNull();
	}

	#endregion

	#region 3000 - Behavior

	/// <summary>
	/// Attempt to set the window title to a specific value.
	/// </summary>
	[Display(Name = "Window title override")]
	[JsonPropertyOrder(3005)]
	public string? WindowTitleOverride
	{
		get => _windowTitleOverride;
		set => _windowTitleOverride = value?.EmptyOrWhiteSpaceToNull();
	}

	#endregion

	#region Validation

	[JsonIgnore]
	public bool IsValid => !this.Validate().Any();

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