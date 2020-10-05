using Polly;
using Polly.Retry;
using System;
using System.Diagnostics;
using System.Linq;

namespace WindowsTerminalQuake.Native
{
	public static class ProcessFactory
	{
		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[]
			{
				TimeSpan.FromMilliseconds(25),
				TimeSpan.FromMilliseconds(50),
				TimeSpan.FromMilliseconds(100),
				TimeSpan.FromMilliseconds(250),
				TimeSpan.FromMilliseconds(500)
			})
		;

		public static Process GetOrCreateWindowsTerminalProcess() => Retry.Execute(() =>
		{
			const string existingProcessName = "WindowsTerminal";

			var process = Process.GetProcessesByName(existingProcessName).FirstOrDefault();
			if (process != null) return process;

			process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "wt",
					UseShellExecute = false,
					RedirectStandardOutput = true,
					WindowStyle = ProcessWindowStyle.Maximized
				}
			};

			process.Start();

			// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available."
			if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Can not access newly started process.");

			return process;
		});
	}
}