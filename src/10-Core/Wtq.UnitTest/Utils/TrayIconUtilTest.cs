using Microsoft.Extensions.Options;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class TrayIconUtilTest
{
	// @formatter:off
	[TestMethod]
	// User: Dark - Should always result in dark icon
	[DataRow(TrayIconStyle.Dark,		OsColorMode.Dark,			"dark.ico")]
	[DataRow(TrayIconStyle.Dark,		OsColorMode.Light,			"dark.ico")]
	[DataRow(TrayIconStyle.Dark,		OsColorMode.Unknown,		"dark.ico")]
	[DataRow(TrayIconStyle.Dark,		OsColorMode.None,			"dark.ico")]
	// User: Light - Should always result in light icon
	[DataRow(TrayIconStyle.Light,		OsColorMode.Dark,			"light.ico")]
	[DataRow(TrayIconStyle.Light,		OsColorMode.Light,			"light.ico")]
	[DataRow(TrayIconStyle.Light,		OsColorMode.Unknown,		"light.ico")]
	[DataRow(TrayIconStyle.Light,		OsColorMode.None,			"light.ico")]
	// User: Auto - Should prefer the opposite of the OS color theme
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Dark,			"light.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Light,			"dark.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Unknown,		"light.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.None,			"light.ico")]
	// User: None - Should act like "Auto"
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Dark,			"light.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Light,			"dark.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.Unknown,		"light.ico")]
	[DataRow(TrayIconStyle.Auto,		OsColorMode.None,			"light.ico")]
	// @formatter:on
	public void TrayIconPathTest(TrayIconStyle trayIconStyle, OsColorMode osColorMode, string expectedTrayIconPath)
	{
		// Arrange
		var platformService = new Mock<IPlatformService>(MockBehavior.Strict);

		// OS color mode
		platformService.Setup(m => m.OsColorMode).Returns(osColorMode);

		// Paths to tray icons
		platformService.Setup(m => m.PathToTrayIconDark).Returns("dark.ico");
		platformService.Setup(m => m.PathToTrayIconLight).Returns("light.ico");

		var opts = Options.Create(new WtqOptions()
		{
			// User configured tray icon style
			TrayIconStyle = trayIconStyle,
		});

		var util = new TrayIconUtil(opts, platformService.Object);

		// Act + Assert
		Assert.AreEqual(expectedTrayIconPath, util.TrayIconPath);
	}
}