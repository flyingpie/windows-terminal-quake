using Wtq.Configuration;
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
	/// Matches option <see cref="WtqAppOptions.FileName"/> to window's <see cref="KWinWtqWindow.DesktopFileName"/> property.
	/// </summary>
	[TestMethod]
	[DataRow("dev.vencord.Vesktop", "dev.vencord.Vesktop")]
	[DataRow("dev.vencord.Vesktop", "DEV.VENCORD.VESKTOP")]
	[DataRow("DEV.VENCORD.VESKTOP", "dev.vencord.Vesktop")]
	public void ByFileName_DesktopFileName_True(string opt, string wnd)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.DesktopFileName = wnd;

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsTrue(res);
	}

	/// <summary>
	/// Matches option <see cref="WtqAppOptions.FileName"/> to window's <see cref="KWinWtqWindow.ResourceClass"/> property.
	/// </summary>
	[TestMethod]
	[DataRow("dev.vencord.Vesktop", "dev.vencord.Vesktop")]
	[DataRow("dev.vencord.Vesktop", "DEV.VENCORD.VESKTOP")]
	[DataRow("DEV.VENCORD.VESKTOP", "dev.vencord.Vesktop")]
	public void ByFileName_ResourceClass_True(string opt, string wnd)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ResourceClass = wnd;

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsTrue(res);
	}

	/// <summary>
	/// Matches option <see cref="WtqAppOptions.FileName"/> to window's <see cref="KWinWtqWindow.ResourceName"/> property.
	/// </summary>
	[TestMethod]
	[DataRow("dev.vencord.Vesktop", "dev.vencord.Vesktop")]
	[DataRow("dev.vencord.Vesktop", "DEV.VENCORD.VESKTOP")]
	[DataRow("DEV.VENCORD.VESKTOP", "dev.vencord.Vesktop")]
	public void ByFileName_ResourceName_True(string opt, string wnd)
	{
		// Arrange
		_opts.FileName = opt;
		_wnd.ResourceName = wnd;

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsTrue(res);
	}

	/// <summary>
	/// Option <see cref="WtqAppOptions.FileName"/> doesn't match anything.
	/// </summary>
	[TestMethod]
	public void ByFileName_False()
	{
		// Arrange
		_opts.FileName = "dev.vencord.Vesktop";

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsFalse(res);
	}

	/// <summary>
	/// Matches by option <see cref="WtqAppOptions.WindowTitle"/>.
	/// </summary>
	[TestMethod]
	[DataRow("dev.vencord.Vesktop", "dev.vencord.Vesktop")]
	[DataRow("dev.vencord.Vesktop", "DEV.VENCORD.VESKTOP")]
	[DataRow("DEV.VENCORD.VESKTOP", "dev.vencord.Vesktop")]
	public void ByWindowTitle_True(string opt, string wnd)
	{
		// Arrange
		_opts.WindowTitle = opt;
		_wnd.Caption = wnd;

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsTrue(res);
	}

	/// <summary>
	/// Matches by option <see cref="WtqAppOptions.WindowTitle"/>, but doesn't hit.
	/// </summary>
	[TestMethod]
	public void ByWindowTitle_False()
	{
		// Arrange
		_opts.WindowTitle = "the-window-title";
		_wnd.Caption = "another-window-title";

		// Act
		var res = _window.Matches(_opts);

		// Assert
		Assert.IsFalse(res);
	}
}