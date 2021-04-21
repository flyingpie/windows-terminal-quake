using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTerminalQuake.Native
{
	public class FocusTracker
	{
		public static event EventHandler OnFocusLost = delegate { };

		private static bool _isRunning;

		public static void FocusGained(Process process)
		{
			Log.Information("Focus gained");

			if (_isRunning) return;

			_isRunning = true;

			Task.Run(async () =>
			{
				while (_isRunning)
				{
					await Task.Delay(TimeSpan.FromMilliseconds(250));

					var main = process.MainWindowHandle;
					if (main != IntPtr.Zero)
					{
						var fg = User32.GetForegroundWindow();
						if (process.MainWindowHandle != fg)
						{
							Log.Information("Focus lost");
							OnFocusLost(null, null);
							_isRunning = false;
							break;
						}
					}
				}
			});
		}

		public static bool GetCurrentFocusWindow()
		{
			var currentWindowHandle = User32.GetForegroundWindow();
			var stringBuilder = new StringBuilder(256);
			if (User32.GetWindowText(currentWindowHandle, stringBuilder, 256) > 0)
			{
				var allProcesses = Process.GetProcesses();
				foreach (var window in allProcesses)
				{
					try
					{
						var mwHandle = window.MainWindowHandle;
						if (mwHandle == currentWindowHandle)
						{
							// This is the current open handles window
							if(Settings.Instance.IgnoreHotKeyWindows.Any(i => i == window.ProcessName))
							{
								return true;
							}
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						// TODO: Better Error Handling
					}
				}
			}

			return false;
		}
	}
}