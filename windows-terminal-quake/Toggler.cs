using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
	public class Toggler : IDisposable
	{
		private Process Process => TerminalProcess.Get(_args);

		private readonly string[] _args;
		private readonly List<int> _registeredHotKeys = new List<int>();

		private readonly IScreenBoundsProvider _scrBoundsProvider = new ScreenBoundsProvider();
		private readonly ITerminalBoundsProvider _termBoundsProvider = new TerminalBoundsProvider();

		public Toggler(string[] args)
		{
			_args = args ?? Array.Empty<string>();

			// Always on top
			if (Settings.Instance.AlwaysOnTop) TopMostWindow.SetTopMost(Process);

			// Hide from taskbar
			var windLong = User32.GetWindowLong(Process.MainWindowHandle, User32.GWL_EX_STYLE);

			User32.SetWindowLong(Process.MainWindowHandle, User32.GWL_EX_STYLE, (windLong | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

			User32.Rect rect = default;
			User32.ShowWindow(Process.MainWindowHandle, NCmdShow.MAXIMIZE);

			var isOpen = true;

			// Register hotkeys
			Settings.Get(s =>
			{
				if (s.Hotkeys == null) return; // Hotkeys not loaded yet

				_registeredHotKeys.ForEach(hk => HotKeyManager.UnregisterHotKey(hk));
				_registeredHotKeys.Clear();

				s.Hotkeys.ForEach(hk =>
				{
					Log.Information($"Registering hot key {hk.Modifiers} + {hk.Key}");
					var reg = HotKeyManager.RegisterHotKey(hk.Key, hk.Modifiers);
					_registeredHotKeys.Add(reg);
				});
			});

			// Hide on focus lost
			FocusTracker.OnFocusLost += (s, a) =>
			{
				if (Settings.Instance.HideOnFocusLost && isOpen)
				{
					isOpen = false;
					Toggle(false, 0);
				}
			};

			// Toggle on hotkey(s)
			HotKeyManager.HotKeyPressed += (s, a) =>
			{
				if (Settings.Instance.DisableOnFullscreenWindow && ActiveWindowIsInFullscreen())
				{
					return;
				}

				Toggle(!isOpen, Settings.Instance.ToggleDurationMs);
				isOpen = !isOpen;
			};

			// Start hidden?
			if (Settings.Instance.StartHidden) Toggle(isOpen = false, 0);
		}

		public void Toggle(bool open, int durationMs)
		{
			var animationFn = AnimationFunction(Settings.Instance.ToggleAnimationType);
			var frameTimeMs = Settings.Instance.ToggleAnimationFrameTimeMs;

			Log.Information(open ? "Open" : "Close");

			if (open) FocusTracker.FocusGained(Process);

			var screen = _scrBoundsProvider.GetTargetScreenBounds();
			User32.ShowWindow(Process.MainWindowHandle, NCmdShow.RESTORE);
			User32.SetForegroundWindow(Process.MainWindowHandle);

			// Used to accurately measure how far we are in the animation
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			TimeSpan ts = stopWatch.Elapsed;
			var curMs = ts.TotalMilliseconds;

			// Run the open/close animation
			while (curMs < durationMs)
			{
				// Asynchronously start the timer for this frame (unless frameTimeMs is 0)
				var frameTimer = (frameTimeMs == 0) ? new TaskAwaiter() : Task.Delay(TimeSpan.FromMilliseconds(frameTimeMs)).GetAwaiter();

				// Render the next frame of animation
				var animationX = open ? (curMs / durationMs) : (1.0 - (curMs / durationMs));
				var bounds = _termBoundsProvider.GetTerminalBounds(screen, animationFn(animationX));
				User32.MoveWindow(Process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

				ts = stopWatch.Elapsed;
				curMs = (double)ts.TotalMilliseconds;

				if (frameTimeMs > 0)
				{
					frameTimer.GetResult(); // Wait for the timer to end
				}
			}
			stopWatch.Stop();

			// To ensure sure we end up in exactly the correct final position
			var finalBounds = _termBoundsProvider.GetTerminalBounds(screen, open ? 1.0 : 0.0);
			User32.MoveWindow(Process.MainWindowHandle, finalBounds.X, finalBounds.Y, finalBounds.Width, finalBounds.Height, true);

			if (open)
			{
				if (Settings.Instance.VerticalScreenCoverage >= 100 && Settings.Instance.HorizontalScreenCoverage >= 100)
				{
					User32.ShowWindow(Process.MainWindowHandle, NCmdShow.MAXIMIZE);
				}
			}
			else
			{
				// Minimize, so the last window gets focus
				User32.ShowWindow(Process.MainWindowHandle, NCmdShow.MINIMIZE);

				// Hide, so the terminal windows doesn't linger on the desktop
				User32.ShowWindow(Process.MainWindowHandle, NCmdShow.HIDE);
			}
		}

		/// <summary>
		/// Returns a mathematical function that can be used for "easing" animations.
		/// Such functions are typically given an X value(representing time) between 0.0 and 1.0,
		/// and return a Y value between 0.0 and 1.0 (representing the position of what we're animating).
		/// </summary>
		/// <param name="type">Name of the easing function; we use the same names as https://easings.net/.</param>
		public Func<double, double> AnimationFunction(AnimationType type)
		{
			switch (type)
			{
				case AnimationType.Linear:
					return (x) => x;

				case AnimationType.EaseInCubic:
					return (x) => Math.Pow(x, 3);

				case AnimationType.EaseOutCubic:
					return (x) => 1.0 - Math.Pow(1.0 - x, 3);

				case AnimationType.EaseInOutSine:
					return (x) => -(Math.Cos(Math.PI * x) - 1.0) / 2.0;

				case AnimationType.EaseInQuart:
					return (x) => Math.Pow(x, 4);

				case AnimationType.EaseOutQuart:
					return (x) => 1.0 - Math.Pow(1.0 - x, 4);

				case AnimationType.EaseInBack:
					return (x) => 2.70158 * x * x * x - 1.70158 * x * x;

				case AnimationType.EaseOutBack:
					return (x) => 1.0 + 2.70158 * Math.Pow(x - 1.0, 3) + 1.70158 * Math.Pow(x - 1.0, 2);

				default:
					Log.Warning("Invalid animation type \"" + type + "\"; falling back to linear.");
					return (x) => x;
			}
		}

		public void Dispose()
		{
			Process.ResetPosition();
		}

		private static bool ActiveWindowIsInFullscreen()
		{
			IntPtr fgWindow = User32.GetForegroundWindow();
			User32.Rect appBounds = new User32.Rect();
			User32.Rect screen = new User32.Rect();
			User32.GetWindowRect(User32.GetDesktopWindow(), ref screen);

			if (fgWindow != User32.GetDesktopWindow() && fgWindow != User32.GetShellWindow())
			{
				if (User32.GetWindowRect(fgWindow, ref appBounds))
				{
					return appBounds.Equals(screen);
				}
			}
			return false;
		}
	}
}