namespace Wtq.Host.Linux.E2ETest;

[TestClass]
public class KonsoleToggleTest
{
	/// <summary>
	/// Passed to Konsole as tab title, so we can find "our" window, and so WTQ won't attach to any other Konsole window that may be open.
	/// </summary>
	private const string WindowTitle = "wtqe2etest";

	private const string AppName = "E2EKonsole";

	/// <summary>
	/// Long-ish animation, so we can reliably observe the window mid-flight.
	/// </summary>
	private const int AnimationDurationMs = 1000;

	/// <summary>
	/// Margin WTQ keeps between the screen edge and the off-screen window (see WtqWindowRectProvider).
	/// </summary>
	private const int OffScreenMargin = 100;

	private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(15);

	private static readonly string WtqJsonc = $$"""
		{
			"Hotkeys": [],
			"Apps": [
				{
					"Name": "{{AppName}}",
					"FileName": "konsole",
					"ArgumentList": [
						{ "Argument": "--separate" },
						{ "Argument": "-p" },
						{ "Argument": "tabtitle={{WindowTitle}}" }
					],

					// Match on the window title only, so we don't grab the user's own Konsole windows.
					"WindowTitle": "{{WindowTitle}}",

					// Never actually pressed, the test invokes the registered KWin shortcut over DBus.
					"Hotkeys": [{ "Modifiers": "Control, Alt, Shift", "Key": "F12" }],

					"AttachMode": "FindOrStart",
					"PreferMonitor": "AtIndex",
					"MonitorIndex": 0,
					"HorizontalScreenCoverage": 50,
					"VerticalScreenCoverage": 50,
					"HorizontalAlign": "Center",
					"VerticalAlign": "Top",
					"VerticalOffset": 0,
					"OffScreenLocations": ["Above"],
					"AnimationDurationMs": {{AnimationDurationMs}},

					// Keep the test deterministic (and the window findable through KRunner).
					"HideOnFocusLost": "Never",
					"Exclusive": "Never",
					"TaskbarIconVisibility": "AlwaysVisible"
				}
			]
		}
		""";

	private readonly KWinTestClient _kwin = new();

	public TestContext TestContext { get; set; } = null!;

	[TestInitialize]
	public async Task SetupAsync()
	{
		// Opted in at this point (see E2ECondition), so fail loudly if the environment isn't usable.
		if (Process.GetProcessesByName("wtq").Length > 0)
		{
			Assert.Fail("Another WTQ instance is running, please close it first.");
		}

		if (!File.Exists(WtqProcess.PathToExe))
		{
			Assert.Fail($"WTQ executable not found at '{WtqProcess.PathToExe}'.");
		}

		await _kwin.ConnectAsync();

		// Clean up leftovers from earlier (crashed) runs.
		await CloseTestWindowsAsync();

		foreach (var name in await GetAppShortcutNamesAsync())
		{
			await _kwin.UnregisterKWinShortcutAsync(name);
		}
	}

	[TestCleanup]
	public async Task CleanupAsync()
	{
		await CloseTestWindowsAsync();
		_kwin.Dispose();
	}

	[TestMethod]
	public async Task SlidesOnAndOffScreen()
	{
		// Expected rects, based on the settings above, and how WTQ calculates window rects.
		var screen = (await _kwin.GetScreenRectsAsync()).First();
		var width = (int)(screen.Width * 0.5f);
		var height = (int)(screen.Height * 0.5f);
		var x = screen.X + (int)Math.Ceiling((screen.Width / 2f) - (width / 2f));

		var onScreen = new Rectangle(x, screen.Y, width, height);

		TestContext.WriteLine($"Screen:    {screen}");
		TestContext.WriteLine($"On-screen: {onScreen}");

		await using var wtq = WtqProcess.Start(WtqJsonc);
		TestContext.WriteLine($"WTQ work dir (settings & logs): {wtq.WorkDir}");

		// Wait for WTQ to load its KWin script and register the app hotkey.
		var shortcut = await WaitForAsync(
			"WTQ to register the app hotkey",
			async () => (await GetAppShortcutNamesAsync()).FirstOrDefault()
		);

		// Stage 1: First toggle, WTQ starts Konsole and moves it onto the screen.
		await _kwin.InvokeKWinShortcutAsync(shortcut);

		var windowId = await WaitForAsync(
			"WTQ to start Konsole",
			async () => (await _kwin.FindWindowIdsAsync(WindowTitle)).FirstOrDefault()
		);

		var first = await WaitForLocationAsync(windowId, onScreen.Location);
		var size = first[^1].Size;
		AssertSize(onScreen.Size, size);

		// When toggling off, WTQ moves the window up by its actual height (plus a margin).
		var offScreen = new Point(onScreen.X, screen.Y - size.Height - OffScreenMargin);
		TestContext.WriteLine($"Off-screen: {offScreen}");

		// Stage 2: Toggle off, window slides up, off the screen.
		await _kwin.InvokeKWinShortcutAsync(shortcut);

		var off = await WaitForLocationAsync(windowId, offScreen);
		AssertSlid(off, from: onScreen.Location, to: offScreen, size);

		// Stage 3: Toggle on again, window slides down, back onto the screen.
		await _kwin.InvokeKWinShortcutAsync(shortcut);

		var on = await WaitForLocationAsync(windowId, onScreen.Location);
		AssertSlid(on, from: offScreen, to: onScreen.Location, size);

		Assert.IsFalse(wtq.HasExited, "WTQ exited unexpectedly.");
	}

	/// <summary>
	/// Apps don't necessarily take the exact size WTQ asks for. Konsole for example snaps to its character grid.<br/>
	/// So we accept a window that's somewhat smaller than requested, but never larger.
	/// </summary>
	private static void AssertSize(Size requested, Size actual)
	{
		const float minRatio = 0.85f;

		Assert.IsTrue(
			actual.Width <= requested.Width && actual.Width >= requested.Width * minRatio,
			$"Window width {actual.Width} not close enough to requested width {requested.Width}."
		);
		Assert.IsTrue(
			actual.Height <= requested.Height && actual.Height >= requested.Height * minRatio,
			$"Window height {actual.Height} not close enough to requested height {requested.Height}."
		);
	}

	/// <summary>
	/// Asserts that the recorded <paramref name="samples"/> show a smooth vertical slide between <paramref name="from"/> and <paramref name="to"/>.
	/// </summary>
	private void AssertSlid(List<Rectangle> samples, Point from, Point to, Size size)
	{
		TestContext.WriteLine(
			$"Slide {from.Y} -> {to.Y}: {string.Join(", ", samples.Select(s => s.Y))}"
		);

		// Horizontal position and size stay put, the window only moves vertically.
		// KWin works with fractional coordinates, so allow for a pixel of rounding.
		foreach (var s in samples)
		{
			Assert.IsLessThanOrEqualTo(
				1,
				Math.Abs(to.X - s.X),
				$"X changed during animation ({s})."
			);
			Assert.AreEqual(size, s.Size, $"Size changed during animation ({s}).");
		}

		// The window must have been observed somewhere in between (i.e., it animated instead of jumped).
		var minY = Math.Min(from.Y, to.Y);
		var maxY = Math.Max(from.Y, to.Y);
		var midFlight = samples.Count(s => s.Y > minY && s.Y < maxY);
		Assert.IsGreaterThanOrEqualTo(
			3,
			midFlight,
			"Expected the window to be observed mid-animation at least a few times."
		);

		// And it must have moved in one direction only.
		// The first sample is where the window rested before the toggle. When toggling on, WTQ first places the window
		// at an off-screen position based on the requested (rather than actual) window height, so skip that sample.
		var dir = Math.Sign(to.Y - from.Y);
		for (var i = 2; i < samples.Count; i++)
		{
			Assert.IsGreaterThanOrEqualTo(
				0,
				Math.Sign(samples[i].Y - samples[i - 1].Y) * dir,
				$"Window moved backwards at sample {i} ({samples[i - 1].Y} -> {samples[i].Y})."
			);
		}
	}

	/// <summary>
	/// Polls the window's rect until its location equals <paramref name="expected"/>, returning all distinct rects observed along the way.<br/>
	/// Then waits for the animation to settle (eased animations reach their final pixel well before they end), and checks the window stays put.
	/// </summary>
	private async Task<List<Rectangle>> WaitForLocationAsync(string windowId, Point expected)
	{
		var samples = new List<Rectangle>();
		var sw = Stopwatch.StartNew();

		while (sw.Elapsed < Timeout)
		{
			var info = await _kwin.GetWindowInfoAsync(windowId);
			Assert.IsNotNull(info, $"Window '{windowId}' disappeared.");

			if (samples.Count == 0 || samples[^1] != info.Rect)
			{
				samples.Add(info.Rect);
			}

			if (info.Rect.Location == expected)
			{
				await Task.Delay(AnimationDurationMs);

				var settled = await _kwin.GetWindowInfoAsync(windowId);
				Assert.AreEqual(
					expected,
					settled?.Rect.Location,
					"Window moved away after reaching its target location."
				);

				return samples;
			}

			await Task.Delay(10);
		}

		Assert.Fail(
			$"Window did not reach {expected} within {Timeout}, observed: {string.Join(", ", samples)}"
		);
		return samples;
	}

	private async Task<string[]> GetAppShortcutNamesAsync() =>
		(await _kwin.GetKWinShortcutNamesAsync())
			.Where(n =>
				n.StartsWith(
					$"wtq_hotkey_app_{AppName.ToLowerInvariant()}_",
					StringComparison.OrdinalIgnoreCase
				)
			)
			.ToArray();

	private async Task CloseTestWindowsAsync()
	{
		foreach (var id in await _kwin.FindWindowIdsAsync(WindowTitle))
		{
			var info = await _kwin.GetWindowInfoAsync(id);
			if (info == null || info.Pid <= 0)
			{
				continue;
			}

			try
			{
				using var p = Process.GetProcessById(info.Pid);
				p.Kill();
				await p.WaitForExitAsync();
			}
			catch (ArgumentException)
			{
				// Already gone.
			}
		}
	}

	private static async Task<T> WaitForAsync<T>(string what, Func<Task<T?>> poll)
	{
		var sw = Stopwatch.StartNew();
		while (sw.Elapsed < Timeout)
		{
			var res = await poll();
			if (res is not null)
			{
				return res;
			}

			await Task.Delay(100);
		}

		throw new TimeoutException($"Timed out waiting for {what}.");
	}
}
