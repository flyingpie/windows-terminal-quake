namespace Wtq.Configuration;

/// <summary>
/// How being the "main-window" affects attaching.<br/>
/// I.e. should WTQ attach to main windows only, non-main windows only, or either.
/// </summary>
public enum MainWindowState
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	[DisplayFlags(IsVisible = false)] // Don't show this in the GUI, it's purely for internal use.
	None = 0,

	/// <summary>
	/// Match main windows only.
	/// </summary>
	[Display(Name = "Main window only")]
	MainWindowOnly,

	/// <summary>
	/// Match non-main windows only.
	/// </summary>
	[Display(Name = "Non-main window only")]
	NonMainWindowOnly,

	/// <summary>
	/// Match main- and non-main windows.
	/// </summary>
	Either,
}