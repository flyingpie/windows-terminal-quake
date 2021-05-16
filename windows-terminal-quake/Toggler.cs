using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.Settings;
using WindowsTerminalQuake.TerminalBoundsProviders;

namespace WindowsTerminalQuake
{
	public class Toggler : IDisposable
	{
		private Process Process => TerminalProcess.Get(_args);

		private readonly string[] _args;
		private readonly List<int> _registeredHotKeys = new List<int>();

		private readonly IAnimationTypeProvider _animTypeProvider = new AnimationTypeProvider();
		private readonly IScreenBoundsProvider _scrBoundsProvider = new ScreenBoundsProvider();
		private ITerminalBoundsProvider _termBoundsProvider = new ResizingTerminalBoundsProvider();

		public Toggler(string[] args)
		{
			_args = args ?? Array.Empty<string>();

			// Always on top
			if (QSettings.Instance.AlwaysOnTop) Process.SetAlwaysOnTop();

			// Taskbar icon visibility
			QSettings.Get(s =>
			{
				Process.ToggleTaskbarIconVisibility(s.TaskbarIconVisibility != TaskBarIconVisibility.AlwaysHidden);
			});

			// Used to keep track of the current toggle state.
			// The terminal is always assumed to be open on app start.
			var isOpen = true;

			// Register hotkeys
			QSettings.Get(s =>
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

			QSettings.Get(s =>
			{
				_termBoundsProvider = s.ToggleMode switch
				{
					ToggleMode.Move => new MovingTerminalBoundsProvider(),
					_ => new ResizingTerminalBoundsProvider()
				};
			});

			// Hide on focus lost
			FocusTracker.OnFocusLost += (s, a) =>
			{
				if (QSettings.Instance.HideOnFocusLost && isOpen)
				{
					isOpen = false;
					Toggle(false, 0);
				}
			};

			// Toggle on hotkey(s)
			HotKeyManager.HotKeyPressed += (s, a) =>
			{
				if (FocusTracker.HotKeySuppressedForCurrentFocusedProcess())
				{
					return;
				}

				if (QSettings.Instance.DisableWhenActiveAppIsInFullscreen && ActiveWindowIsInFullscreen())
				{
					return;
				}

				isOpen = !isOpen;

				Toggle(isOpen, QSettings.Instance.ToggleDurationMs);
			};

			// Start hidden?
			if (QSettings.Instance.StartHidden) Toggle(isOpen = false, 0);
			// Otherwise, call toggle once to setup the correct size and position
			else Toggle(isOpen = true, 0);
		}

		public void Toggle(bool open, int durationMs)
		{
			var animationFn = _animTypeProvider.GetAnimationFunction();
			var frameTimeMs = QSettings.Instance.ToggleAnimationFrameTimeMs;

			Log.Information(open ? "Open" : "Close");

			// Notify focus tracker
			if (open) FocusTracker.FocusGained(Process);

			Process.BringToForeground();

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

				var intermediateBounds = _termBoundsProvider.GetTerminalBounds(screen, animationFn(linearProgress));

				Process.MoveWindow(bounds: intermediateBounds);

				if (frameTimeMs > 0)
				{
					frameTimer.GetResult(); // Wait for the timer to end
				}
			}
			stopwatch.Stop();

			// To ensure sure we end up in exactly the correct final position
			var finalBounds = _termBoundsProvider.GetTerminalBounds(screen, open ? 1.0 : 0.0);
			Process.MoveWindow(bounds: finalBounds);

			if (open)
			{
				// If vertical- and horizontal screen coverage is set to 100, maximize the window to make it actually fullscreen
				if (QSettings.Instance.MaximizeAfterToggle && QSettings.Instance.VerticalScreenCoverage >= 100 && QSettings.Instance.HorizontalScreenCoverage >= 100)
				{
					Process.SetWindowState(WindowShowStyle.Maximize);
				}
			}
			else
			{
				// Minimize first, so the last window gets focus
				Process.SetWindowState(WindowShowStyle.Minimize);

				// Then hide, so the terminal windows doesn't linger on the desktop
				if (QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.AlwaysHidden || QSettings.Instance.TaskbarIconVisibility == TaskBarIconVisibility.WhenTerminalVisible)
					Process.SetWindowState(WindowShowStyle.Hide);
			}
		}

		public void Dispose()
		{
			Process.ResetBounds();
			Process.ToggleTaskbarIconVisibility(true);
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