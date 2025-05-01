using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
[Display(Name = ":material-application-outline: App options")]
public sealed class WtqAppOptions : WtqSharedOptions, IValidatableObject
{
	private ICollection<ProcessArgument> _argumentList = [];
	private ICollection<HotkeyOptions> _hotkeys = [];
	private string? _fileName;
	private string? _processName;
	private string? _windowClass;
	private string? _windowTitle;
	private string? _windowTitleOverride;

	/// <summary>
	/// Used to refer from the app options object back to the global one, for cascading.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	[JsonIgnore]
	public WtqOptions Global { get; set; } = null!;

	#region 1000 - App

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads.<br/>
	/// Appears in logs.
	/// </summary>
	/// <example>
	/// <code>
	/// {
	/// 	"Name": "Terminal",
	/// 	// ...
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.App)]
	[JsonPropertyOrder(1001)]
	[NotNull]
	[Required]
	public string? Name { get; set; }

	/// <summary>
	/// One or more keyboard shortcuts that toggle in- and out this particular app.
	/// </summary>
	[Display(GroupName = Gn.App)]
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
	/// <remarks>
	/// See the "Examples" page in the GUI for, well, examples.
	/// </remarks>
	[Display(GroupName = Gn.Process, Name = "Filename")]
	[ExampleValue("wt")]
	[JsonPropertyOrder(2001)]
	[Required]
	public string? FileName
	{
		get => _fileName;
		set => _fileName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Apps sometimes have <strong>process names</strong> different from their <strong>filenames</strong>.
	/// This field can be used to look for the process name in such cases. Windows Terminal is an
	/// example, with filename <strong>wt</strong>, and process name <strong>WindowsTerminal</strong>.
	/// </summary>
	/// <example>
	/// <code>
	/// {
	/// 	// Using with Windows Terminal requires both "Filename" and "ProcessName".
	/// 	"Apps": {
	/// 		"Filename": "wt",
	/// 		"ProcessName": "WindowsTerminal"
	/// 	}
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.Process, Name = "Process name")]
	[JsonPropertyOrder(2003)]
	public string? ProcessName
	{
		get => _processName;
		set => _processName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <strong>AttachMode</strong> that starts the app.
	/// </summary>
	[Display(GroupName = Gn.Process)]
	[JsonPropertyOrder(2004)]
	public string? Arguments { get; set; }

	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// Note that this only applies when using an <strong>AttachMode</strong> that starts the app.
	/// </summary>
	/// <example>
	/// <code>
	/// {
	/// 	"Apps": [
	/// 		{
	/// 			"ArgumentList": [
	/// 				"--allow-screencapture",
	/// 				"--debug-info",
	/// 			],
	/// 			// ...
	/// 		}
	/// 	]
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.Process, Name = "Argument list")]
	[JsonPropertyOrder(2004)]
	public ICollection<ProcessArgument> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	[Display(GroupName = Gn.Process, Name = "Window class")]
	[ExampleValue("ApplicationFrameWindow")]
	[JsonPropertyOrder(2006)]
	public string? WindowClass
	{
		get => _windowClass;
		set => _windowClass = value?.EmptyOrWhiteSpaceToNull();
	}

	[Display(GroupName = Gn.Process, Name = "Window title")]
	[ExampleValue("Mozilla Firefox - WhatsApp")]
	[JsonPropertyOrder(2007)]
	public string? WindowTitle
	{
		get => _windowTitle;
		set => _windowTitle = value?.EmptyOrWhiteSpaceToNull();
	}

	#endregion

	#region 3000 - Behavior

	/// <summary>
	/// <para>
	/// Attempt to set the window title to a specific value.
	/// </para>
	/// <para>
	/// Useful for cases where multiple programs control window placement (such as when
	/// using WTQ together with a window manager) and the window title can be used to
	/// opt-out in the other program.
	/// </para>
	/// </summary>
	/// <remarks>
	/// Note that this doesn't work for all windows, as it depends on factors like the app's GUI kit.
	/// </remarks>
	[Display(GroupName = Gn.Behavior, Name = "Window title override")]
	[JsonPropertyOrder(3005)]
	public string? WindowTitleOverride
	{
		get => _windowTitleOverride;
		set => _windowTitleOverride = value?.EmptyOrWhiteSpaceToNull();
	}

	#endregion

	/// <summary>
	/// Called right after the options are loaded from file.
	/// </summary>
	public void OnPostConfigure(WtqOptions options)
	{
		Global = Guard.Against.Null(options);
	}

	/// <summary>
	/// Called before the settings are persisted to a file.
	/// </summary>
	public void PrepareForSave()
	{
		foreach (var hk in Hotkeys.Where(hk => hk.IsEmpty).ToList()) // Explicit ToList() since we're modifying it from within the loop.
		{
			Hotkeys.Remove(hk);
		}
	}

	protected override IEnumerable<ValidationResult> OnValidate(ValidationContext validationContext)
	{
		// App name.
		if (string.IsNullOrWhiteSpace(Name))
		{
			yield return new("A name is required.", [nameof(Name)]);
		}

		// Require a filename, process name or window title.
		if (string.IsNullOrWhiteSpace(FileName) && string.IsNullOrWhiteSpace(ProcessName) && string.IsNullOrWhiteSpace(WindowTitle))
		{
			yield return new("Either a <strong>filename</strong>, a <strong>process name</strong> or a <strong>window title</strong> needs to be set.", [nameof(FileName), nameof(ProcessName), nameof(WindowTitle)]);
		}

		// Require at least 1 hotkey.
		if (Hotkeys.Count == 0)
		{
			yield return new("Specify at least 1 hotkey.", [nameof(Hotkeys)]);
		}
	}

	public override string ToString() => Name;
}