using Polly;
using Polly.Retry;
using System.Drawing;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace System.Diagnostics
{
	public static class ProcessExtensions
	{
		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) })
		;

		/// <summary>
		/// Sets the position and size of the process' main window.
		/// </summary>
		public static void MoveWindow(this Process process, Rectangle bounds, bool repaint = true)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			User32.MoveWindow
			(
				hWnd: process.MainWindowHandle,
				X: bounds.X,
				Y: bounds.Y,
				nWidth: bounds.Width,
				nHeight: bounds.Height,
				bRepaint: repaint
			);
		}

		/// <summary>
		/// Reset position and size of the specified process' window to be centered on the primary monitor.
		/// </summary>
		public static void ResetBounds(this Process process)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			var bounds = Screen.PrimaryScreen.Bounds;

			bounds.X += 100;
			bounds.Y += 100;
			bounds.Width -= 200;
			bounds.Height -= 200;

			// Reset position
			process.MoveWindow(bounds);

			// Restore window
			process.SetWindowState(WindowShowStyle.Restore);
		}

		/// <summary>
		/// Make sure the window is always the top-most one.
		/// </summary>
		public static void SetAlwaysOnTop(this Process process)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			Retry.Execute(() =>
			{
				if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Process handle zero");

				var isSet = User32.SetWindowPos(process.MainWindowHandle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.TOPMOST_FLAGS);
				if (!isSet) throw new Exception("Could not set window top most");
			});
		}

		/// <summary>
		/// Bring the process' main window to the foreground.
		/// </summary>
		public static void BringToForeground(this Process process)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			process.SetWindowState(WindowShowStyle.Restore);
			User32.SetForegroundWindow(process.MainWindowHandle);
		}

		/// <summary>
		/// Makes the entire window of the specified process transparent.
		/// </summary>
		/// <param name="transparency">Desired transparency, value between 0 (invisible) to 100 (opaque).</param>
		public static void SetTransparency(this Process process, int transparency)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			Retry.Execute(() =>
			{
				if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Process handle zero");

				// Get original window properties
				var props = User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE);

				// Add "WS_EX_LAYERED"-flag (required for transparency).
				User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, props | User32.WS_EX_LAYERED);

				// Set transparency
				var isSet = User32.SetLayeredWindowAttributes(process.MainWindowHandle, 0, (byte)Math.Ceiling(255f / 100f * transparency), User32.LWA_ALPHA);
				if (!isSet) throw new Exception("Could not set window opacity");
			});
		}

		public static void SetWindowState(this Process process, WindowShowStyle state)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			User32.ShowWindow(process.MainWindowHandle, state);
		}

		/// <summary>
		/// Hides- or shows the taskbar icon of the specified process.
		/// </summary>
		public static void ToggleTaskbarIconVisibility(this Process process, bool isVisible)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			// Get handle to the main window
			var handle = process.MainWindowHandle;

			// Get current window properties
			var props = User32.GetWindowLong(handle, User32.GWL_EX_STYLE);

			if (isVisible)
			{
				// Show
				User32.SetWindowLong(handle, User32.GWL_EX_STYLE, (props | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);
			}
			else
			{
				// Hide
				User32.SetWindowLong(handle, User32.GWL_EX_STYLE, (props | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);
			}
		}
	}
}