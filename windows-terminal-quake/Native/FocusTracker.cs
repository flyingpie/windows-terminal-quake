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
		public static bool HotKeySupressedForCurrentFocusedProcess()
		{
			var currentWindowHandle = User32.GetForegroundWindow();
			
			List<Process> processes = new List<Process>();
			foreach (var suppressedProcess in Settings.Instance.SuppressHotKeyForProcesses)
			{
				// need to remove the .exe
				var processNameClean = suppressedProcess.Remove(
					suppressedProcess.IndexOf(".exe", StringComparison.InvariantCultureIgnoreCase)
					);
				var processesFound = Process.GetProcessesByName(processNameClean);
				if (processesFound.Any())
					processes.AddRange(processesFound);
				// found no processes with the names for supressed -> return false (no ignore)
				else 
					return false;
			}
			
			// Check if the current window was this one
			var currentWindow = processes.FirstOrDefault(x => x.MainWindowHandle == currentWindowHandle) ?? null;
			// There wil only ever be one "current window" (as in from foreground)
			string mainModuleName = "";
			try
			{
				if (currentWindow == null) return false;
				if (currentWindow.MainModule != null) mainModuleName = currentWindow.MainModule.ModuleName;
			}
			catch (Exception e)
			{
				Log.Error("Unable to determine module name from current focus window", e);
				return false;
			}
			if (!String.IsNullOrEmpty(mainModuleName))
			{
				if(Settings.Instance.SuppressHotKeyForProcesses.Any(
					i => String.Compare(mainModuleName, i, comparisonType: StringComparison.InvariantCultureIgnoreCase) == 0))
				{
					// This window should ignore the hotkey
					return true;
				}
			}
			return false;
		}
	}
}