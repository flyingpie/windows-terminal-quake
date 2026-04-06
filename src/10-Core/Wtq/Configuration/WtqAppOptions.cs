using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

/// <summary>
/// Defines the options for a single toggleable app (eg. Windows Terminal, some other terminal, a file browser, etc.).
/// </summary>
[Display(Name = ":material-application-outline: App options")]
public sealed class WtqAppOptions : WtqSharedOptions
{
	private ICollection<ProcessArgument> _argumentList = [];
	private ICollection<HotkeyOptions> _hotkeys = [];
	private string? _fileName;
	private string? _processName;
	private string? _windowClass;
	private string? _windowTitle;
	private string? _windowTitleOverride;
	private string? _workingDirectory;

	/// <summary>
	/// Used to refer from the app options object back to the global one, for cascading.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	[JsonIgnore]
	public WtqOptions Global { get; set; } = null!;

	#region 1000 - App

	/// <summary>
	/// A logical name for the app, used to identify it across config reloads, and it appears in logs.
	/// </summary>
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
	/// The <b>filename</b> to use when starting a new process for the app. If no <b>process name</b> is set,
	/// the value of this property is also used to match against the names of running processes (to find the process to attach to).<br/>
	/// <br/>
	/// E.g. <b>notepad</b>, <b>dolphin</b>, etc.
	/// <br/>
	/// Note that (if the app is not in the OS PATH), you can also put absolute paths in here,
	/// or specify the working directory through <b>working directory</b>.<br/>
	/// <br/>
	/// See the <b>Examples</b> page for, well, examples.
	/// </summary>
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
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// <br/>
	/// Note that this only applies when using an <b>AttachMode</b> that starts the app.<br/>
	/// <br/>
	/// This is mostly here for backward-compatibility reasons. Prefer <b>argument list</b>,
	/// which handles complex arguments and escaping better.
	/// </summary>
	[Display(GroupName = Gn.Process)]
	[JsonPropertyOrder(2002)]
	public string? Arguments { get; set; }

	/// <summary>
	/// Command-line arguments that should be passed to the app when it's started.<br/>
	/// <br/> 
	/// Note that this only applies when using an <b>AttachMode</b> that starts the app (i.e., when WTQ actually starts the app).<br/>
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Argument list")]
	[JsonPropertyOrder(2003)]
	public ICollection<ProcessArgument> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	/// <summary>
	/// Working directory when starting a new process.<br/>
	/// <br/>
	/// Useful if the <b>filename</b> isn't available through PATH.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Working directory")]
	[JsonPropertyOrder(2004)]
	public string? WorkingDirectory
	{
		get => _workingDirectory;
		set => _workingDirectory = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Apps sometimes have <b>process names</b> different from their <b>filenames</b>.<br/>
	/// <br/>
	/// This field can be used to look for the process name in such cases. Windows Terminal is an
	/// example, with filename <b>wt</b>, and process name <b>WindowsTerminal</b>.<br/>
	/// <br/>
	/// Supports regular expressions.<br/>
	/// <br/>
	/// Also see the <b>Examples</b> page for more cases where this is relevant, and the <b>Windows</b> page on how to find values.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Process name")]
	[JsonPropertyOrder(2005)]
	public string? ProcessName
	{
		get => _processName;
		set => _processName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// (Windows only) Matches only "main" windows; the initial window a process spawns.<br/>
	/// <br/>
	/// When a process spawns multiple windows, 1 is usually the "main" window. In many cases, this is also the window
	/// that you'd want to use WTQ with. Non-main-windows are usually child windows like popups and such.<br/>
	/// <br/>
	/// When the other (non-main) windows cannot be easily differentiated from the main window (for example through
	/// the window class or -title), the "is-main-window"-property can be very useful to home in on the desired window.<br/>
	/// <br/>
	/// A common example of an app where this would <b>not</b> help, is a browser, that can spawn tons of windows on the same process name.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Main window")]
	[DefaultValue(MainWindowState.Either)]
	[JsonPropertyOrder(2006)]
	public MainWindowState? MainWindow { get; set; }

	/// <summary>
	/// (Windows only) Matches windows based on their Win32 Window Class.<br/>
	/// <br/>
	/// Supports regular expressions.
	/// <br/>
	/// A common example of an app where this would <b>not</b> help, is a browser, that can spawn tons of windows on the same process name.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Window class")]
	[ExampleValue("^ApplicationFrameWindow$")]
	[JsonPropertyOrder(2007)]
	public string? WindowClass
	{
		get => _windowClass;
		set => _windowClass = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// Match windows based on their title (sometimes referred to as "caption").<br/>
	/// <br/>
	/// Supports regular expressions.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Window title")]
	[ExampleValue("^Mozilla Firefox - WhatsApp$")]
	[JsonPropertyOrder(2008)]
	public string? WindowTitle
	{
		get => _windowTitle;
		set => _windowTitle = value?.EmptyOrWhiteSpaceToNull();
	}

	#endregion

	#region 3000 - Behavior

	/// <summary>
	/// Attempt to set the window title to a specific value.<br/>
	/// <br/>
	/// Useful for cases where multiple programs control window placement (such as when using WTQ together with a window manager)
	/// and the window title can be used to opt-out in the other program.<br/>
	/// <br/>
	/// Note that this doesn't work for all windows, as it depends on factors like the app's GUI toolkit.
	/// </summary>
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
		// Explicit ToList() since we're modifying it from within the loop.
		foreach (var hk in Hotkeys.Where(hk => hk.Sequence.IsEmpty).ToList())
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
			yield return new("Either a <b>filename</b>, a <b>process name</b> or a <b>window title</b> needs to be set.", [nameof(FileName), nameof(ProcessName), nameof(WindowTitle)]);
		}

		// Require at least 1 hotkey.
		if (Hotkeys.Count == 0)
		{
			yield return new("Specify at least 1 hotkey.", [nameof(Hotkeys)]);
		}
	}

	public override string ToString() => Name;
}