namespace Wtq.Core.UnitTest.Configuration;

[TestClass]
public class WtqAppOptionsExtensionsTest
{
	private WtqOptions _gbl = new WtqOptions();
	private WtqAppOptions _app = new WtqAppOptions();

	[TestInitialize]
	public void Setup()
	{
		_gbl = new WtqOptions();
		_app = new WtqAppOptions
		{
			Global = _gbl,
		};
	}

	#region HideOnFocusLost

	[TestMethod]
	public void GetHideOnFocusLost_Default()
	{
		Assert.AreEqual(HideOnFocusLost.Always, _app.GetHideOnFocusLost());
	}

	[TestMethod]
	public void GetHideOnFocusLost_Global()
	{
		_gbl.HideOnFocusLost = HideOnFocusLost.UnlessFocusChangedScreen;

		Assert.AreEqual(HideOnFocusLost.UnlessFocusChangedScreen, _app.GetHideOnFocusLost());
	}

	[TestMethod]
	public void GetHideOnFocusLost_App()
	{
		_app.HideOnFocusLost = HideOnFocusLost.UnlessFocusChangedScreen;

		Assert.AreEqual(HideOnFocusLost.UnlessFocusChangedScreen, _app.GetHideOnFocusLost());
	}

	[TestMethod]
	public void GetHideOnFocusLost_GlobalAndApp()
	{
		_gbl.HideOnFocusLost = HideOnFocusLost.Never;
		_app.HideOnFocusLost = HideOnFocusLost.UnlessFocusChangedScreen;

		Assert.AreEqual(HideOnFocusLost.UnlessFocusChangedScreen, _app.GetHideOnFocusLost());
	}

	#endregion

	#region MainWindow

	[TestMethod]
	public void GetMainWindow_Default()
	{
		Assert.AreEqual(MainWindowState.Either, _app.GetMainWindow());
	}

	[TestMethod]
	public void GetMainWindow_Either()
	{
		_app.MainWindow = MainWindowState.Either;

		Assert.AreEqual(MainWindowState.Either, _app.GetMainWindow());
	}

	[TestMethod]
	public void GetMainWindow_MainWindowOnly()
	{
		_app.MainWindow = MainWindowState.MainWindowOnly;

		Assert.AreEqual(MainWindowState.MainWindowOnly, _app.GetMainWindow());
	}

	[TestMethod]
	public void GetMainWindow_NonMainWindowOnly()
	{
		_app.MainWindow = MainWindowState.NonMainWindowOnly;

		Assert.AreEqual(MainWindowState.NonMainWindowOnly, _app.GetMainWindow());
	}

	#endregion
}