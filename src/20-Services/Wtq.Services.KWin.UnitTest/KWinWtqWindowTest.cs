using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin.UnitTest;

[TestClass]
public class KWinWtqWindowTest
{
	private readonly Mock<IKWinClient> _kwin = new();
	private KWinWindow _wnd = new();
	private WtqAppOptions _opts = new();

	private KWinWtqWindow _window = null!;

	[TestInitialize]
	public void Setup()
	{
		_window = new KWinWtqWindow(_kwin.Object, _wnd);
	}

	/// <summary>
	/// Matching using the "FileName" option, against the "DesktopFileName" property.
	/// </summary>
	[TestMethod]

	// Exact
	[DataRow("FileName", "FileName", true)]
	[DataRow("FileName", "FILENAME", true)]
	[DataRow("FileName", "filename", true)]

	// No match
	[DataRow("FileName", null, false)]
	[DataRow("FileName", "", false)]
	[DataRow("FileName", " ", false)]
	[DataRow("FileName", "OtherName", false)]
	public void ByFileName_DesktopFileName(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.DesktopFileName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	/// <summary>
	/// Matching using the "FileName" option, against the "ResourceClass" property.
	/// </summary>
	[TestMethod]

	// Exact
	[DataRow("ResourceClass", "ResourceClass", true)]
	[DataRow("resourceclass", "RESOURCECLASS", true)]
	[DataRow("RESOURCECLASS", "resourceclass", true)]

	// No match
	[DataRow("ResourceClass", null, false)]
	[DataRow("ResourceClass", "", false)]
	[DataRow("ResourceClass", " ", false)]
	[DataRow("ResourceClass", "OtherName", false)]
	public void ByFileName_ResourceClass(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ResourceClass = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	/// <summary>
	/// Matching using the "FileName" option, against the "ResourceName" property.
	/// </summary>
	[TestMethod]

	// Exact
	[DataRow("ResourceName", "ResourceName", true)]
	[DataRow("resourcename", "RESOURCENAME", true)]
	[DataRow("RESOURCENAME", "resourcename", true)]

	// No match
	[DataRow("ResourceName", null, false)]
	[DataRow("ResourceName", "", false)]
	[DataRow("ResourceName", " ", false)]
	[DataRow("ResourceName", "OtherName", false)]
	public void ByFileName_ResourceName(string opt, string wnd, bool isMatch)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ResourceName = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}

	/// <summary>
	/// Matching using the "WindowTitle" option, against the "Caption" property.
	/// </summary>
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
		_wnd.Caption = wnd;

		// Act + Assert
		Assert.AreEqual(isMatch, _window.Matches(_opts));
	}
}