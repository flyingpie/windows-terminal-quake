using Wtq.Services.AnimationTypeProviders;
using Wtq.Services.AppBoundsProviders;
using Wtq.Services.ScreenBoundsProviders;

namespace Wtq.Services.Apps;

public class WtqAppToggleService(
	IAnimationProvider animTypeProvider,
	IScreenBoundsProvider scrBoundsProvider,
	IAppBoundsProvider termBoundsProvider)
	: IWtqAppToggleService
{
	private readonly IAnimationProvider _animTypeProvider = Guard.Against.Null(animTypeProvider);
	private readonly ILogger _log = Log.For<WtqAppToggleService>();
	private readonly IScreenBoundsProvider _scrBoundsProvider = Guard.Against.Null(scrBoundsProvider);
	private readonly IAppBoundsProvider _termBoundsProvider = Guard.Against.Null(termBoundsProvider);

	[SuppressMessage("Major Bug", "S2583:Conditionally executed code should be reachable", Justification = "MvdO: Frame time will be configurable.")]
	public async Task ToggleAsync(WtqApp app, bool open, int durationMs)
	{
		Guard.Against.Null(app);

		// TODO: Change it so that this method doesn't even get called if we don't have a valid process.
		if (!app.IsActive)
		{
			_log.LogWarning("Attempted to toggle inactive app '{App}'", app);
			return;
		}

		var animationFn = _animTypeProvider.GetAnimationFunction();

		// var frameTimeMs = QSettings.Instance.ToggleAnimationFrameTimeMs;
		var frameTimeMs = 25;

		_log.LogInformation(open ? "Open" : "Close");

		// Notify focus tracker
		// if (open) FocusTracker.FocusGained(Process);

		// TODO: Move to WtqService?
		if (open)
		{
			app.BringToForeground();
		}

		var screen = _scrBoundsProvider.GetTargetScreenBounds(app);

		// Used to accurately measure how far we are in the animation
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		// Run the open/close animation
		while (stopwatch.ElapsedMilliseconds < durationMs)
		{
			var deltaMs = (float)stopwatch.ElapsedMilliseconds;

			// Asynchronously start the timer for this frame (unless frameTimeMs is 0)
			var frameTimer = frameTimeMs == 0
				? Task.CompletedTask
				: Task.Delay(TimeSpan.FromMilliseconds(frameTimeMs));

			// Render the next frame of animation
			var linearProgress = open
				? deltaMs / durationMs
				: 1.0 - (deltaMs / durationMs);

			var wndRect = app.GetWindowRect();
			var intermediateBounds = _termBoundsProvider.GetNextAppBounds(app, open, screen, wndRect, animationFn(linearProgress));

			app.MoveWindow(intermediateBounds);

#pragma warning disable S2589 // Boolean expressions should not be gratuitous // MvdO: Frame time will be configurable.
			if (frameTimeMs > 0)
			{
				await frameTimer.ConfigureAwait(false); // Wait for the timer to end
			}
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
		}

		stopwatch.Stop();

		// To ensure we end up in exactly the correct final position
		var wndRect2 = app.GetWindowRect();
		var finalBounds = _termBoundsProvider.GetNextAppBounds(app, open, screen, wndRect2, open ? 1.0 : 0.0);
		app.MoveWindow(rect: finalBounds);

		_log.LogInformation("Moved window to {Bounds}", finalBounds);

		if (open)
		{
			// TODO: Only do this in cases where we want the app to disappear? Toggling window state causes flickering.
			//// If vertical- and horizontal screen coverage is set to 100, maximize the window to make it actually fullscreen
			// if (QSettings.Instance.MaximizeAfterToggle && QSettings.Instance.VerticalScreenCoverage >= 100 && QSettings.Instance.HorizontalScreenCoverage >= 100)
			// {
			// Process.SetWindowState(WindowShowStyle.Maximize);
			// }
			app.BringToForeground();
		}
		else
		{
			// TODO: Only do this in cases where we want the app to disappear? Toggling window state causes flickering.
			// Minimize first, so the last window gets focus
			// process.SetWindowState(WindowShowStyle.Minimize);

			// Then hide, so the terminal windows doesn't linger on the desktop
			// if (QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.AlwaysHidden || QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.WhenTerminalVisible)
			// process.SetWindowState(WindowShowStyle.Hide);
		}
	}

	// private static bool ActiveWindowIsInFullscreen()
	// {
	// IntPtr fgWindow = User32.GetForegroundWindow();
	// var appBounds = new Rect();
	// var screen = new Rect();
	// User32.GetWindowRect(User32.GetDesktopWindow(), ref screen);

	// if (fgWindow != User32.GetDesktopWindow() && fgWindow != User32.GetShellWindow())
	// {
	// if (User32.GetWindowRect(fgWindow, ref appBounds))
	// {
	// return appBounds.Equals(screen);
	// }
	// }
	// return false;
	// }
}