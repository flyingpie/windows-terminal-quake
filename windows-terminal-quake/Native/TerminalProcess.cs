using Polly;
using Polly.Retry;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using WindowsTerminalQuake.Settings;

namespace WindowsTerminalQuake.Native
{
	/// <summary>
	/// Wrapper around the Windows Terminal process. Contains stuff to actually capture the process,
	/// which turns out to be tricky in some cases.
	/// </summary>
	public static class TerminalProcess
	{
		/// <summary>
		/// Returns the instance of the running Windows Terminal. Creates one if none is running yet.
		/// </summary>
		/// <param name="args">Any command-line arguments to pass to the terminal process if we're starting it.</param>
		public static Process Get(string[] args)
		{
			return Retry.Execute(() =>
			{
				if (_isExitting) return _process!;

				if (_process == null || _process.HasExited)
				{
					_process = GetOrCreate(args);
				}

				return _process;
			});
		}

		private static readonly RetryPolicy Retry = Policy
			.Handle<Exception>()
			.WaitAndRetry(new[]
				{
					TimeSpan.FromMilliseconds(25),
					TimeSpan.FromMilliseconds(50),
					TimeSpan.FromMilliseconds(100),
					TimeSpan.FromMilliseconds(250),
					TimeSpan.FromMilliseconds(500)
				},
				onRetry: (ex, t) => Log.Error($"Error creating process: '{ex.Message}'"));

		private static Process? _process;

		private static List<Action> _onExit = new List<Action>();

		private static bool _isExitting;

		public static void OnExit(Action action)
		{
			_onExit.Add(action);
		}

		private static void FireOnExit()
		{
			_isExitting = true;

			_onExit.ForEach(a => a());
		}

		private static Process GetOrCreate(string[] args)
		{
			const string existingProcessName = "WindowsTerminal";

			var process = Process.GetProcessesByName(existingProcessName).FirstOrDefault();
			if (process == null)
			{
				process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = QSettings.Instance.WindowsTerminalCommand,
						Arguments = string.Join(" ", args),
						UseShellExecute = false
					}
				};

				try
				{
					process.Start();
					process.WaitForInputIdle();
				}
				catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
				{
					throw new Exception($"Could not find the Windows Terminal exe at '{QSettings.Instance.WindowsTerminalCommand}'. Make sure it is installed, and see '{nameof(QSettings.Instance.WindowsTerminalCommand)}' setting for more information.");
				}

				// After starting the process, just throw an exception so the process search gets restarted.
				// The "wt.exe" process does some stuff to ultimately fire up a "WindowsTerminal" process, so we can't actually use the Process instance we just created.
				throw new Exception($"Started process");
			}

			// Try the "nice way" of waiting for the process to become ready
			process.Refresh();
			process.WaitForInputIdle();

			Log.Information(
				$"Got process with id '{process.Id}' and name '{process.ProcessName}' and title '{process.MainWindowTitle}'.");

			// Make sure the process has not exited
			if (process.HasExited) throw new Exception($"Process existing.");

			// Make sure we can access the main window handle
			// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available."
			if (process.MainWindowHandle == IntPtr.Zero) throw new Exception("Main window handle not accessible.");

			// Make sure the process name equals "WindowsTerminal", otherwise WT might still be starting
			if (process.ProcessName != "WindowsTerminal") throw new Exception("Process name is not 'WindowsTerminal' yet.");

			// We need a proper window title before we can continue
			if (process.MainWindowTitle == "")
				throw new Exception($"Process still has temporary '' (empty) window title.");

			// This is a way-too-specific check to further ensure the WT process is ready
			if (process.MainWindowTitle == "DesktopWindowXamlSource")
				throw new Exception($"Process still has temporary 'DesktopWindowXamlSource' window title.");

			process.EnableRaisingEvents = true;
			process.Exited += (s, a) => FireOnExit();

			return process;
		}
	}
}