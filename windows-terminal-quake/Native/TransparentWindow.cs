using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;

namespace WindowsTerminalQuake.Native
{
	public class TransparentWindow
	{
		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) })
		;

		public static void SetTransparent(Process process, int transparency)
		{
			Retry.Execute(() =>
			{
				if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Process handle zero");

				var old = User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE);
				User32.ThrowIfError();
				var old2 = User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, old | User32.WS_EX_LAYERED);

				var isSet = User32.SetLayeredWindowAttributes(process.MainWindowHandle, 0, (byte)Math.Ceiling(255f / 100f * transparency), User32.LWA_ALPHA);
				if (!isSet) throw new Exception("Could not set window opacity");
			});
		}
	}
}