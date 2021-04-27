using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace System.Diagnostics
{
	public static class ProcessExtensions
	{
		public static void ResetPosition(this Process process)
		{
			if (process == null) throw new ArgumentNullException(nameof(process));

			var bounds = Screen.PrimaryScreen.Bounds;

			// Restore taskbar icon
			var windLong = User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE);
			User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (windLong | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

			// Reset position
			User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

			// Restore window
			User32.ShowWindow(process.MainWindowHandle, NCmdShow.MAXIMIZE);
		}
	}
}