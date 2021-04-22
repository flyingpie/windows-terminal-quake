using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTerminalQuake.Native
{
	/// <summary>
	/// Watches the specified <see cref="Process"/> to see if it still has focus, and fires an event when it doesn't.
	///
	/// Useful to hide the terminal when another window gets focus (if enabled).
	/// </summary>
	public class FocusTracker
	{
		public static event EventHandler OnFocusLost = delegate { };

		private static bool _isRunning;

		public static void FocusGained(Process process)
		{
			Log.Information("Focus gained");

			if (_isRunning) return;

			_isRunning = true;

			// Run a loop when the process gets focus
			Task.Run(async () =>
			{
				while (_isRunning)
				{
					// Wait a bit between iterations
					await Task.Delay(TimeSpan.FromMilliseconds(250));

					// Get the window handle
					var main = process.MainWindowHandle;
					if (main == IntPtr.Zero)
					{
						continue;
					}

					// See if the foreground window is still the specified one
					var fg = User32.GetForegroundWindow();
					if (process.MainWindowHandle != fg)
					{
						// If the foreground window is different to the one we're watching, we lost focus
						Log.Information("Focus lost");

						OnFocusLost(null, null);
						_isRunning = false;
						break;
					}
				}
			});
		}

		// Checks to see if the current window in focus should ignore hotkey
		public static bool CheckFocusIgnoreHotKey()
		{
			var currentWindowHandle = User32.GetForegroundWindow();
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
								// This window should ignore the hotkey
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
			return false;
		}
	}
}