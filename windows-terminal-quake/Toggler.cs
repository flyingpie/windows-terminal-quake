﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake
{
	public class Toggler : IDisposable
	{
		private Process _process => TerminalProcess.Get(_args);

		private string[] _args;
		private readonly List<int> _registeredHotKeys = new List<int>();

		public Toggler(string[] args)
		{
			_args = args;

			// Always on top
			if (Settings.Instance.AlwaysOnTop) TopMostWindow.SetTopMost(_process);

			// Hide from taskbar
			var windLong = User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE);
			User32.ThrowIfError();

			User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE, (windLong | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

			User32.Rect rect = default;
			User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);

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
				Toggle(!isOpen, Settings.Instance.ToggleDurationMs);
				isOpen = !isOpen;
			};

			// Start hidden?
			if (Settings.Instance.StartHidden) Toggle(isOpen = false, 0);
		}

		public void Toggle(bool open, int durationMs) {
			var animationFn = AnimationFunction(Settings.Instance.ToggleAnimationType);
			var frameTimeMs = Settings.Instance.ToggleAnimationFrameTimeMs;

			Log.Information(open?"Open":"Close");
			if (open) {
				FocusTracker.FocusGained(_process);
			}
			var screen = GetScreenWithCursor();
			User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
			User32.SetForegroundWindow(_process.MainWindowHandle);

			// Used to accurately measure how far we are in the animation
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			TimeSpan ts = stopWatch.Elapsed;
			var curMs = ts.TotalMilliseconds;

			// Run the open/close animation
			while (curMs < durationMs)
			{
				// Asynchronously start the timer for this frame (unless frameTimeMs is 0)
				TaskAwaiter frameTimer = (frameTimeMs == 0)? new TaskAwaiter(): Task.Delay(TimeSpan.FromMilliseconds(frameTimeMs)).GetAwaiter();
				
				// Render the next frame of animation
				var animationX = open ? (curMs / durationMs) : (1.0 - (curMs / durationMs));
				var bounds = GetBounds(screen, animationFn(animationX));
				User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);
				User32.ThrowIfError();

				ts = stopWatch.Elapsed;
				curMs = (double)ts.TotalMilliseconds;
				if (frameTimeMs != 0) {
					frameTimer.GetResult(); // Wait for the timer to end
				}
			}
			stopWatch.Stop();

			// To ensure sure we end up in exactly the correct final position
			var finalBounds = GetBounds(screen, open?1.0:0.0);
			User32.MoveWindow(_process.MainWindowHandle, finalBounds.X, finalBounds.Y, finalBounds.Width, finalBounds.Height, true);
			User32.ThrowIfError();


			if (open) {
				if (Settings.Instance.VerticalScreenCoverage >= 100 && Settings.Instance.HorizontalScreenCoverage >= 100)
				{
					User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);
				}
			} else {
				// Minimize, so the last window gets focus
				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);

				// Hide, so the terminal windows doesn't linger on the desktop
				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
			}
		}

		/**
		 * <summary>Determine the window size & position during a toggle animation.</summary>
		 * <param name="animationPosition">value between 0.0 and 1.0 to indicate the desired position of the window;
		 *	at 0.0 the window is completely hidden; at 1.0 it is fully visible/opened.</param>
		 */
		public Rectangle GetBounds(Screen screen, double animationPosition)
		{			
			if (screen == null) throw new ArgumentNullException(nameof(screen));

			var settings = Settings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

			var bounds = screen.Bounds;

			var scrWidth = bounds.Width;
			var horWidthPct = (float)settings.HorizontalScreenCoverage;

			var horWidth = (int)Math.Ceiling(scrWidth / 100f * horWidthPct);
			var x = 0;

			switch (settings.HorizontalAlign)
			{
				case HorizontalAlign.Left:
					x = bounds.X;
					break;

				case HorizontalAlign.Right:
					x = bounds.X + (bounds.Width - horWidth);
					break;

				case HorizontalAlign.Center:
				default:
					x = bounds.X + (int)Math.Ceiling(scrWidth / 2f - horWidth / 2f);
					break;
			}

			bounds.Height = (int)Math.Ceiling((bounds.Height / 100f) * settings.VerticalScreenCoverage);
			bounds.Height += settings.VerticalOffset;

			return new Rectangle(
				x,
				bounds.Y + -bounds.Height + (int)Math.Round(bounds.Height * animationPosition) + settings.VerticalOffset,
				horWidth,
				bounds.Height
			);
		}

		/**
		 * <summary>Returns a mathematical function that can be used for "easing" animations.
		 * Such functions are typically given an X value (representing time) between 0.0 and 1.0,
		 * and return a Y value between 0.0 and 1.0 (representing the position of what we're animating).</summary>
		 * <param name="name">Name of the easing function; we use the same names as https://easings.net/ </param>
		 */
		public Func<double, double> AnimationFunction(string name)
		{
			switch (name)
			{
				case "linear":
					return (x) => x;
				case "easeInCubic":
					return (x) => Math.Pow(x, 3);
				case "easeOutCubic":
					return (x) => 1.0 - Math.Pow(1.0 - x, 3);
				case "easeInOutSine":
					return (x) => -(Math.Cos(Math.PI * x) - 1.0) / 2.0;
				case "easeInQuart":
					return (x) => Math.Pow(x, 4);
				case "easeOutQuart":
					return (x) => 1.0 - Math.Pow(1.0 - x, 4);
				case "easeInBack":
					return (x) => 2.70158 * x * x * x - 1.70158 * x * x;
				case "easeOutBack":
					return (x) => 1.0 + 2.70158 * Math.Pow(x - 1.0, 3) + 1.70158 * Math.Pow(x - 1.0, 2);
				default:
					Log.Warning("Invalid animation type \"" + name + "\"; falling back to linear.");
					return (x) => x;
			}
		}

		public void Dispose()
		{
			ResetTerminal(_process);
		}

		private static Screen GetScreenWithCursor()
		{
			var settings = Settings.Instance;
			if (settings == null) return Screen.PrimaryScreen; // Should not happen

			var scr = Screen.AllScreens;

			switch (settings.PreferMonitor)
			{
				// At Index
				case PreferMonitor.AtIndex:
					// Make sure the monitor index is within bounds
					if (settings.MonitorIndex < 0)
					{
						TrayIcon.Instance.Notify(ToolTipIcon.Warning, $"Setting '{nameof(Settings.Instance.MonitorIndex)}' must be greater than or equal to 0.");
						return Screen.PrimaryScreen;
					}

					if (settings.MonitorIndex >= scr.Length)
					{
						TrayIcon.Instance.Notify(ToolTipIcon.Warning, $"Setting '{nameof(Settings.Instance.MonitorIndex)}' ({settings.MonitorIndex}) must be less than the monitor count ({scr.Length}).");
						return Screen.PrimaryScreen;
					}

					return scr[settings.MonitorIndex];

				// Primary
				case PreferMonitor.Primary:
					return Screen.PrimaryScreen;

				// With Cursor
				default:
				case PreferMonitor.WithCursor:
					return Screen.AllScreens
						.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position))
						?? Screen.PrimaryScreen
					;
			}
		}

		private static void ResetTerminal(Process process)
		{
			var bounds = GetScreenWithCursor().Bounds;

			// Restore taskbar icon
			var windLong = User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE);
			User32.ThrowIfError();
			User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (windLong | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

			// Reset position
			User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);
			User32.ThrowIfError();

			// Restore window
			User32.ShowWindow(process.MainWindowHandle, NCmdShow.MAXIMIZE);
		}
	}
}