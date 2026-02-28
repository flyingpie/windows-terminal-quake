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
	/// <para>
	/// A logical name for the app, used to identify it across config reloads.
	/// </para>
	/// <para>
	/// Appears in logs.
	/// </para>
	/// </summary>
	/// <example>
	/// <code>
	/// {
	///   "Name": "Terminal",
	///   // ...
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
	/// <para>
	/// The <strong>filename</strong> to use when starting a new process for the app.
	/// </para>
	/// <para>
	/// E.g. <strong>notepad</strong>, <strong>dolphin</strong>, etc.
	/// </para>
	/// <para>
	/// Note that you can also put absolute paths in here.
	/// </para>
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
	/// <para>
	/// Command-line arguments that should be passed to the app when it's started.
	/// </para>
	/// <para>
	/// Note that this only applies when using an <strong>AttachMode</strong> that starts the app.
	/// </para>
	/// </summary>
	[Display(GroupName = Gn.Process)]
	[JsonPropertyOrder(2002)]
	public string? Arguments { get; set; }

	/// <summary>
	/// <para>
	/// Command-line arguments that should be passed to the app when it's started.
	/// </para>
	/// <para>
	/// Note that this only applies when using an <strong>AttachMode</strong> that starts the app.
	/// </para>
	/// </summary>
	/// <example>
	/// <code>
	/// {
	///   "Apps": [
	///     {
	///       "ArgumentList": [
	///         "--allow-screencapture",
	///         "--debug-info",
	///       ],
	///       // ...
	///     }
	///   ]
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.Process, Name = "Argument list")]
	[JsonPropertyOrder(2003)]
	public ICollection<ProcessArgument> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	/// <summary>
	/// <para>
	/// Working directory when starting a new process.
	/// </para>
	/// <para>
	/// Useful if the <strong>filename</strong> isn't available through PATH.
	/// </para>
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Working directory")]
	[JsonPropertyOrder(2004)]
	public string? WorkingDirectory
	{
		get => _workingDirectory;
		set => _workingDirectory = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// <para>
	/// Apps sometimes have <strong>process names</strong> different from their <strong>filenames</strong>.
	/// This field can be used to look for the process name in such cases. Windows Terminal is an
	/// example, with filename <strong>wt</strong>, and process name <strong>WindowsTerminal</strong>.
	/// </para>
	/// <para>
	/// Supports regular expressions.
	/// </para>
	/// </summary>
	/// <example>
	/// <code>
	/// {
	///   // Using with Windows Terminal requires both "Filename" and "ProcessName".
	///   "Apps": {
	///     "Filename": "wt",
	///     "ProcessName": "^WindowsTerminal$"
	///   }
	/// }
	/// </code>
	/// </example>
	[Display(GroupName = Gn.Process, Name = "Process name")]
	[JsonPropertyOrder(2005)]
	public string? ProcessName
	{
		get => _processName;
		set => _processName = value?.EmptyOrWhiteSpaceToNull();
	}

	/// <summary>
	/// <para>
	/// (Windows only) Matches only "main" windows; the initial window a process spawns.
	/// </para>
	/// <para>
	/// When a process spawns multiple windows, 1 is usually the "main" window. In many cases, this is also the window
	/// that you'd want to use WTQ with. Non-main-windows are usually child windows like popups and such.
	/// When the other (non-main) windows cannot be easily differentiated from the main window (for example through
	/// the window class or -title), the "is-main-window"-property can be very useful to home in on the desired window.
	/// </para>
	/// <para>
	/// A common example of an app where this would <strong>not</strong> help, is a browser, that can spawn tons of windows on the same process name.
	/// </para>
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Main window")]
	[DefaultValue(MainWindowState.Either)]
	[JsonPropertyOrder(2006)]
	public MainWindowState? MainWindow { get; set; }

	/// <summary>
	/// <para>
	/// (Windows only) Matches windows based on their Win32 Window Class.
	/// </para>
	/// <para>
	/// Supports regular expressions.
	/// </para>
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
	/// <para>
	/// Match windows based on their title (sometimes referred to as "caption").
	/// </para>
	/// <para>
	/// Supports regular expressions.
	/// </para>
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