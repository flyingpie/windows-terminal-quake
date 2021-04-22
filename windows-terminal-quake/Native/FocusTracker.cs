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

		public static bool CheckFocusIgnoreHotKey()
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

							string mainModuleName;
							try
							{
								mainModuleName = window.MainModule.ModuleName;
							}
							catch (Exception e)
							{
								Log.Error("Unable to determine module name from current focus window");
								return false;
							}
							// This is the current open handles window
							if (!String.IsNullOrEmpty(mainModuleName))
							{
								if(Settings.Instance.IgnoreHotKeyWindows.Any(i => i == mainModuleName))
								{
									return true;
								}
							}
							return false;
						}
					}
					catch (Exception e)
					{
						Log.Error("Exception occured when attempting to determine current window focus module");
					}
				}
			}
			return false;
		}
	}
}