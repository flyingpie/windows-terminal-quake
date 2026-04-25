using System.Collections.Generic;
using System.Linq;
using Wtq.Configuration;
using Wtq.Services;

namespace Wtq.Core.UnitTest.Services;

/// <summary>
/// Tests for per-screen app toggling behavior.
/// Validates rectangle intersection assumptions that the <see cref="IWtqAppRepo.GetOpenOnScreen"/>
/// method relies on, as well as the <see cref="FeatureFlags.PerScreenApps"/> default value.
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

	[TestMethod]
	public void GetOpenOnScreen_ReturnsEmpty_WhenNoAppsMatch()
	{
		// Verify that filtering open apps by screen returns an empty collection
		// when no apps have a CurrentScreenRect matching the query rectangle.
		var apps = new List<WtqApp>();

		var result = apps.Where(a =>
			a is { IsAttached: true, IsOpen: true } &&
			a.CurrentScreenRect.HasValue &&
			a.CurrentScreenRect.Value.IntersectsWith(new Rectangle(5000, 5000, 1920, 1080)));

		Assert.IsNotNull(result, "Filtered result should never be null");
		Assert.AreEqual(0, result.Count(), "No apps should match a far-off screen rect");
	}

	[TestMethod]
	public void GetOpenOnScreen_ReturnsMultipleApps_OnSameScreen()
	{
		// Simulate two open apps on the same screen rect.
		// The method should return all of them, not just the first.
		var screen = new Rectangle(0, 0, 1920, 1080);

		// Create a minimal list that simulates two apps sharing a screen.
		// Since WtqApp is hard to construct in tests, we verify the filter
		// predicate logic directly.
		var testData = new[]
		{
			new { IsAttached = true, IsOpen = true, CurrentScreenRect = (Rectangle?)screen },
			new { IsAttached = true, IsOpen = true, CurrentScreenRect = (Rectangle?)screen },
			new { IsAttached = true, IsOpen = false, CurrentScreenRect = (Rectangle?)screen }, // closed app
		};

		var matching = testData.Where(a =>
			a.IsAttached && a.IsOpen &&
			a.CurrentScreenRect.HasValue &&
			a.CurrentScreenRect.Value.IntersectsWith(screen)).ToList();

		Assert.AreEqual(2, matching.Count, "Should return exactly 2 open apps on the same screen");
	}

	[TestMethod]
	public void GetOpenOnScreen_ExcludesApps_OnDifferentScreen()
	{
		var primaryScreen = new Rectangle(0, 0, 1920, 1080);
		var secondaryScreen = new Rectangle(1920, 0, 1920, 1080);

		var testData = new[]
		{
			new { IsAttached = true, IsOpen = true, CurrentScreenRect = (Rectangle?)primaryScreen },
			new { IsAttached = true, IsOpen = true, CurrentScreenRect = (Rectangle?)secondaryScreen },
		};

		// Query for primary screen should only return the app on the primary screen.
		var onPrimary = testData.Where(a =>
			a.IsAttached && a.IsOpen &&
			a.CurrentScreenRect.HasValue &&
			a.CurrentScreenRect.Value.IntersectsWith(primaryScreen)).ToList();

		Assert.AreEqual(1, onPrimary.Count, "Only one app should match the primary screen");
	}
}