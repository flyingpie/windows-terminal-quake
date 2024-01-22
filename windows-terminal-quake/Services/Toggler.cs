using System.Diagnostics;
using System.Runtime.CompilerServices;
using Wtq.Native;
using Wtq.Native.Win32;
using Wtq.Services.AnimationTypeProviders;
using Wtq.Services.ScreenBoundsProviders;
using Wtq.Services.TerminalBoundsProviders;

namespace Wtq.Services;

public class Toggler(
	IAnimationTypeProvider animTypeProvider,
	IScreenBoundsProvider scrBoundsProvider,
	ITerminalBoundsProvider termBoundsProvider)
{
	private readonly ILogger _log = Log.For<Toggler>();

	private readonly IAnimationTypeProvider _animTypeProvider = animTypeProvider ?? throw new ArgumentNullException(nameof(animTypeProvider));
	private readonly IScreenBoundsProvider _scrBoundsProvider = scrBoundsProvider ?? throw new ArgumentNullException(nameof(scrBoundsProvider));
	private readonly ITerminalBoundsProvider _termBoundsProvider = termBoundsProvider ?? throw new ArgumentNullException(nameof(termBoundsProvider));

	private bool _isOpen = true;

	public void Toggle(Process process)
	{
		_isOpen = !_isOpen;

		Toggle(process, _isOpen, durationMs: 200);
	}

	public void Toggle(Process process, bool open, int durationMs)
	{
		// TODO: Change it so that this method doesn't even get called if we don't have a valid process.
		if (process == null)
		{
			return;
		}

		var animationFn = _animTypeProvider.GetAnimationFunction();
		//var frameTimeMs = QSettings.Instance.ToggleAnimationFrameTimeMs;
		var frameTimeMs = 25;

		_log.LogInformation(open ? "Open" : "Close");

		// Notify focus tracker
		//if (open) FocusTracker.FocusGained(Process);

		process.BringToForeground();

		var screen = _scrBoundsProvider.GetTargetScreenBounds();

		// Used to accurately measure how far we are in the animation
		var stopwatch = new Stopwatch();
		stopwatch.Start();

		// Run the open/close animation
		while (stopwatch.ElapsedMilliseconds < durationMs)
		{
			var deltaMs = (float)stopwatch.ElapsedMilliseconds;

			// Asynchronously start the timer for this frame (unless frameTimeMs is 0)
			var frameTimer = (frameTimeMs == 0)
				? new TaskAwaiter()
				: Task.Delay(TimeSpan.FromMilliseconds(frameTimeMs)).GetAwaiter()
			;

			// Render the next frame of animation
			var linearProgress = open
				? (deltaMs / durationMs)
				: (1.0 - (deltaMs / durationMs))
			;

			var intermediateBounds = _termBoundsProvider.GetTerminalBounds(open, screen, process.GetBounds(), animationFn(linearProgress));

			process.MoveWindow(bounds: intermediateBounds);

			if (frameTimeMs > 0)
			{
				frameTimer.GetResult(); // Wait for the timer to end
			}
		}
		stopwatch.Stop();

		// To ensure we end up in exactly the correct final position
		var finalBounds = _termBoundsProvider.GetTerminalBounds(open, screen, process.GetBounds(), open ? 1.0 : 0.0);
		process.MoveWindow(bounds: finalBounds);

		_log.LogInformation("Moved window to {Bounds}", finalBounds);

		if (open)
		{
			//// If vertical- and horizontal screen coverage is set to 100, maximize the window to make it actually fullscreen
			//if (QSettings.Instance.MaximizeAfterToggle && QSettings.Instance.VerticalScreenCoverage >= 100 && QSettings.Instance.HorizontalScreenCoverage >= 100)
			//{
			//	Process.SetWindowState(WindowShowStyle.Maximize);
			//}
		}
		else
		{
			// Minimize first, so the last window gets focus
			process.SetWindowState(WindowShowStyle.Minimize);

			// Then hide, so the terminal windows doesn't linger on the desktop
//			if (QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.AlwaysHidden || QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.WhenTerminalVisible)
				process.SetWindowState(WindowShowStyle.Hide);
		}
	}

	//private static bool ActiveWindowIsInFullscreen()
	//{
	//	IntPtr fgWindow = User32.GetForegroundWindow();
	//	var appBounds = new Rect();
	//	var screen = new Rect();
	//	User32.GetWindowRect(User32.GetDesktopWindow(), ref screen);

	//	if (fgWindow != User32.GetDesktopWindow() && fgWindow != User32.GetShellWindow())
	//	{
	//		if (User32.GetWindowRect(fgWindow, ref appBounds))
	//		{
	//			return appBounds.Equals(screen);
	//		}
	//	}
	//	return false;
	//}
}