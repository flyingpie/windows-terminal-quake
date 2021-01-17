using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
			User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

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

		public void Toggle(bool open, int durationMs)
		{
			// StepDelayMS needs to be at least 15, due to the resolution of Task.Delay()
			var stepDelayMs = Math.Max(15, Settings.Instance.ToggleAnimationFrameTimeMs);

			var stepCount = durationMs / stepDelayMs;

			var screen = GetScreenWithCursor();

			// Close
			if (!open)
			{
				Log.Information("Close");

				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
				User32.SetForegroundWindow(_process.MainWindowHandle);

				for (int i = stepCount - 1; i >= 0; i--)
				{
					var bounds = GetBounds(screen, stepCount, i);

					User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

					Task.Delay(TimeSpan.FromMilliseconds(stepDelayMs)).GetAwaiter().GetResult();
				}

				// Minimize, so the last window gets focus
				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);

				// Hide, so the terminal windows doesn't linger on the desktop
				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
			}
			// Open
			else
			{
				Log.Information("Open");
				FocusTracker.FocusGained(_process);

				User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
				User32.SetForegroundWindow(_process.MainWindowHandle);

				for (int i = 1; i <= stepCount; i++)
				{
					var bounds = GetBounds(screen, stepCount, i);
					User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

					Task.Delay(TimeSpan.FromMilliseconds(stepDelayMs)).GetAwaiter().GetResult();
				}

				if (Settings.Instance.VerticalScreenCoverage >= 100 && Settings.Instance.HorizontalScreenCoverage >= 100)
				{
					User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);
				}
			}
		}

		public Rectangle GetBounds(Screen screen, int stepCount, int step)
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
				bounds.Y + -bounds.Height + (bounds.Height / stepCount * step) + settings.VerticalOffset,
				horWidth,
				bounds.Height
			);
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
			User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

			// Reset position
			User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

			// Restore window
			User32.ShowWindow(process.MainWindowHandle, NCmdShow.MAXIMIZE);
		}
	}
}