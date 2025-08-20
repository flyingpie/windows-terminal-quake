namespace Wtq.Services.Win32v2.UnitTest;

[TestClass]
public class Win32WtqWindowTest
{
	private readonly Mock<IWin32> _win32 = new();
	private Win32Window _wnd = new(() => false);
	private WtqAppOptions _opts = new();

	private Win32WtqWindow _window = null!;

	[TestInitialize]
	public void Setup()
	{
		_window = new Win32WtqWindow(_win32.Object, _wnd);
	}

	/// <summary>
	/// Only a file name is specified, which is then used to match on process name.
	/// </summary>
	[TestMethod]

	// Exact
	[DataRow("ProcessExplorer", "ProcessExplorer", true)]
	[DataRow("ProcessExplorer", "processexplorer", true)]
	[DataRow("processexplorer", "ProcessExplorer", true)]

	// No match
	[DataRow("procexp", "ProcessExplorer", false)]
	public void ByFileName_ProcessName(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ProcessName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	[TestMethod]

	// Either
	[DataRow(MainWindowState.Either, true, true)]
	[DataRow(MainWindowState.Either, false, true)]

	// Main window only
	[DataRow(MainWindowState.MainWindowOnly, true, true)]
	[DataRow(MainWindowState.MainWindowOnly, false, false)]

	// Non-main window only
	[DataRow(MainWindowState.NonMainWindowOnly, true, false)]
	[DataRow(MainWindowState.NonMainWindowOnly, false, true)]
	public void ByMainWindow(MainWindowState opt, bool isMainWindow, bool isMatch)
	{
		// Arrange
		_opts.MainWindow = opt;
		_wnd.IsMainWindow = isMainWindow;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	/// <summary>
	/// An explicit process name is specified, so we can use that to match.
	/// </summary>
	[TestMethod]

	// Exact
	[DataRow("ProcessExplorer", "ProcessExplorer", true)]
	[DataRow("ProcessExplorer", "processexplorer", true)]
	[DataRow("processexplorer", "ProcessExplorer", true)]

	// No match
	[DataRow("procexp", "ProcessExplorer", false)]

	// Regex - Match
	[DataRow("ProcessExp", "ProcessExplorer", true)] // Without explicit regex symbols, acts as "contains".
	[DataRow("ProcessExp.*", "ProcessExplorer", true)]
	[DataRow(".*cessExp.*", "ProcessExplorer", true)]
	[DataRow(".rocessExplorer", "ProcessExplorer", true)]

	// Regex - No match
	[DataRow("TheProcessName", null, false)]
	[DataRow("TheProcessName", "", false)]
	[DataRow("TheProcessName", " ", false)]
	[DataRow(".*ExpProc.*", "ProcessExplorer", false)] // No match
	public void ByProcessName(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = "the-file-name"; // Should not be used in matching.
		_opts.ProcessName = opt;
		_wnd.ProcessName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	[TestMethod]

	// Exact
	[DataRow("TheWindowClass", "TheWindowClass", true)]
	[DataRow("TheWindowClass", "thewindowclass", true)]
	[DataRow("thewindowclass", "TheWindowClass", true)]

	// No match
	[DataRow("thewndclass", "TheWindowClass", false)]

	// Regex - Match
	[DataRow("TheWindowC", "TheWindowClass", true)] // Without explicit regex symbols, acts as "contains".
	[DataRow("TheWindowC.*", "TheWindowClass", true)]
	[DataRow(".*WindowCl.*", "TheWindowClass", true)]
	[DataRow(".WindowClass", "TheWindowClass", true)]

	// Regex - No match
	[DataRow("TheWindowClass", null, false)]
	[DataRow("TheWindowClass", "", false)]
	[DataRow("TheWindowClass", " ", false)]
	[DataRow(".*ClassWindow.*", "TheWindowClass", false)]
	public void ByWindowClass(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.WindowClass = opt;
		_wnd.WindowClass = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	[TestMethod]

	// Exact
	[DataRow("TheWindowTitle", "TheWindowTitle", true)]
	[DataRow("TheWindowTitle", "thewindowtitle", true)]
	[DataRow("thewindowtitle", "TheWindowTitle", true)]

	// No match
	[DataRow("thewndtitle", "TheWindowTitle", false)]

	// Regex - Match
	[DataRow("TheWindowT", "TheWindowTitle", true)] // Without explicit regex symbols, acts as "contains".
	[DataRow("TheWindowT.*", "TheWindowTitle", true)]
	[DataRow(".*WindowT.*", "TheWindowTitle", true)]
	[DataRow(".WindowTitle", "TheWindowTitle", true)]

	// Regex - No match
	[DataRow("TheWindowTitle", null, false)]
	[DataRow("TheWindowTitle", "", false)]
	[DataRow("TheWindowTitle", " ", false)]
	[DataRow(".*TitleWindow.*", "TheWindowTitle", false)]
	public void ByWindowTitle(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.WindowTitle = opt;
		_wnd.WindowCaption = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}
}