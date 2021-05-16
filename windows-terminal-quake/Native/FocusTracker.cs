using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsTerminalQuake.Settings;
using Serilog.Core;

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
		public static bool HotKeySuppressedForCurrentFocusedProcess()
		{
			var currentWindowHandle = User32.GetForegroundWindow();
			
			List<Process> processes = new List<Process>();
			foreach (var suppressedProcess in QSettings.Instance.SuppressHotkeyForProcesses)
			{
				// Clean up the name of the processes
				var processNameClean = suppressedProcess.Remove(
					suppressedProcess.IndexOf(".exe", StringComparison.InvariantCultureIgnoreCase)
					);
				
				var processesFound = Process.GetProcessesByName(processNameClean);
				if (processesFound.Any())
					processes.AddRange(processesFound);
				else 
					return false;
			}
			
			// find if the foreground (current window) is in the list of suppressed processes
			var currentWindow = processes.FirstOrDefault(x => x.MainWindowHandle == currentWindowHandle) ?? null;
			try
			{
				if (currentWindow == null) return false; // current window was not in the suppressed processes
				if (currentWindow.MainModule?.ModuleName != null) return true; // The foreground window has been matched with a suppressed process
			}
			catch (Exception e)
			{
				Log.Error("Unable to determine module name from current focus window", e);
				return false;
			}
			return false;
		}
	}
}