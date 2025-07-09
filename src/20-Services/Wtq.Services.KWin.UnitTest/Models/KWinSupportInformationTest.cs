using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin.UnitTest.Models;

[TestClass]
public class KWinSupportInformationTest
{
	[TestMethod]
	public void ParseTest_SingleScreen()
	{
		// Arrange
		var supportInfoStr = """
			actionLeft: 0

			Screens
			=======
			Active screen follows mouse:  yes
			Number of Screens: 1

			Screen 0:
			---------
			Name: eDP-1
			Enabled: 1
			Geometry: 0,0,1920x1080
			Scale: 1
			Refresh Rate: 60049
			Adaptive Sync: incapable

			Compositing
			===========
			Compositing is active
			""";

		// Act
		var res = KWinSupportInformation.Parse(supportInfoStr);

		// Assert
		Assert.IsNotNull(res);
		Assert.IsNotNull(res.Screens);
		Assert.AreEqual(1, res.Screens.Count);
	}

	[TestMethod]
	public void ParseTest_MultiScreen()
	{
		// Arrange
		var supportInfoStr = """
			actionBottomLeft: 0
			actionLeft: 0

			Screens
			=======
			Active screen follows mouse:  yes
			Number of Screens: 3

			Screen 0:
			---------
			Name: eDP-1
			Enabled: 1
			Geometry: 0,360,1920x1080
			Scale: 1
			Refresh Rate: 60003
			Adaptive Sync: incapable
			Screen 1:
			---------
			Name: DP-2
			Enabled: 1
			Geometry: 1920,0,2560x1440
			Scale: 1
			Refresh Rate: 59951
			Adaptive Sync: incapable
			Screen 2:
			---------
			Name: HDMI-A-1
			Enabled: 1
			Geometry: 4480,0,2560x1440
			Scale: 1
			Refresh Rate: 59951
			Adaptive Sync: incapable

			Compositing
			===========
			Compositing is active
			""";

		// Act
		var res = KWinSupportInformation.Parse(supportInfoStr);

		// Assert
		Assert.IsNotNull(res);
		Assert.IsNotNull(res.Screens);
		Assert.AreEqual(3, res.Screens.Count);

		var scr1 = res.Screens.FirstOrDefault(s => s.Name == "eDP-1");
		Assert.IsNotNull(scr1);

		var scr2 = res.Screens.FirstOrDefault(s => s.Name == "DP-2");
		Assert.IsNotNull(scr2);

		var scr3 = res.Screens.FirstOrDefault(s => s.Name == "HDMI-A-1");
		Assert.IsNotNull(scr3);
	}
}