using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WindowsTerminalQuake.Native
{
	public class TransparentWindow
	{
		public static void SetTransparent(Process process, int transparency)
		{
			Task.Run(async () =>
			{
				for (int i = 0; i < 3; i++)
				{
					if (process.MainWindowHandle == IntPtr.Zero)
					{
						await Task.Delay(TimeSpan.FromSeconds(1));
					}
				}

				for (int i = 0; i < 3; i++)
				{
					var old = User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE);
					var old2 = User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, old | User32.WS_EX_LAYERED);

					var x = User32.SetLayeredWindowAttributes(process.MainWindowHandle, 0, (byte)Math.Ceiling(255f / 100f * transparency), User32.LWA_ALPHA);
					if (x) break;

					await Task.Delay(TimeSpan.FromSeconds(1));
				}
			});
		}
	}
}