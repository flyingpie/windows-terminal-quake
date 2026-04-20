using Wtq.Configuration;

namespace Wtq.Core.UnitTest.Services;

/// <summary>
/// Tests for the per-screen app toggling feature.
/// The core logic relies on <see cref="Rectangle.IntersectsWith(Rectangle)"/> to determine
/// whether a WTQ app on one screen would conflict with a toggle on another screen.
/// These tests validate the screen-intersection assumptions.
/// </summary>
[TestClass]
public class WtqAppRepoPerScreenTest
{
	[TestMethod]
	public void PerScreenApps_DefaultValue_IsTrue()
	{
		var flags = new FeatureFlags();
		Assert.IsTrue(flags.PerScreenApps);
	}

	[TestMethod]
	public void AdjacentHorizontalMonitorRects_DoNotIntersect()
	{
		// Common dual-monitor setup: primary at (0,0), secondary immediately to the right.
		var primaryScreen = new Rectangle(0, 0, 1920, 1080);
		var secondaryScreen = new Rectangle(1920, 0, 1920, 1080);

		Assert.IsFalse(primaryScreen.IntersectsWith(secondaryScreen),
			"Adjacent horizontal monitors should not intersect, so apps on each screen are independent");
	}

	[TestMethod]
	public void OverlappingMonitorRects_DoIntersect()
	{
		// Some monitor setups have slight overlap (e.g., 10px overlap).
		var screenA = new Rectangle(0, 0, 1930, 1080);
		var screenB = new Rectangle(1920, 0, 1920, 1080);

		Assert.IsTrue(screenA.IntersectsWith(screenB),
			"Overlapping monitors should intersect, so apps would conflict");
	}

	[TestMethod]
	public void IdenticalScreenRects_Intersect()
	{
		// An app on a screen should match a query for that same screen.
		var screen = new Rectangle(0, 0, 1920, 1080);

		Assert.IsTrue(screen.IntersectsWith(screen),
			"A screen should intersect with itself");
	}

	[TestMethod]
	public void VerticalStackedMonitorRects_DoNotIntersect()
	{
		// Vertical stacked monitors (one above the other).
		var topScreen = new Rectangle(0, -1080, 1920, 1080);
		var bottomScreen = new Rectangle(0, 0, 1920, 1080);

		Assert.IsFalse(topScreen.IntersectsWith(bottomScreen),
			"Vertically stacked adjacent monitors should not intersect");
	}

	[TestMethod]
	public void TripleHorizontalSetup_NoCrossScreenIntersection()
	{
		// Left monitor | Primary | Right monitor
		var leftScreen = new Rectangle(-1920, 0, 1920, 1080);
		var primaryScreen = new Rectangle(0, 0, 1920, 1080);
		var rightScreen = new Rectangle(1920, 0, 1920, 1080);

		// Primary does not intersect with either side monitor.
		Assert.IsFalse(primaryScreen.IntersectsWith(leftScreen),
			"Primary screen should not intersect with left screen");
		Assert.IsFalse(primaryScreen.IntersectsWith(rightScreen),
			"Primary screen should not intersect with right screen");

		// Side monitors don't intersect each other.
		Assert.IsFalse(leftScreen.IntersectsWith(rightScreen),
			"Left and right screens should not intersect");
	}
}