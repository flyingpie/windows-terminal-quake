using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Serilog.Core;

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

		// Checks to see if the current window in focus should ignore hotkey
		public static bool HotKeySuppressedForCurrentFocusedProcess()
		{
			var currentWindowHandle = User32.GetForegroundWindow();
			
			List<Process> processes = new List<Process>();
			foreach (var suppressedProcess in Settings.Instance.SuppressHotKeyForProcesses)
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