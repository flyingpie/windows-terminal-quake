using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;

namespace WindowsTerminalQuake.Native
{
	public static class TopMostWindow
	{
		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[] { TimeSpan.FromMilliseconds(250), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) })
		;

		public static void SetTopMost(Process process)
		{
			Retry.Execute(() =>
			{
				if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Process handle zero");

				var isSet = User32.SetWindowPos(process.MainWindowHandle, User32.HWND_TOPMOST, 0, 0, 0, 0, User32.TOPMOST_FLAGS);
				if (!isSet) throw new Exception("Could not set window top most");
			});
		}
	}
}