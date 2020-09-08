using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;
using Serilog;

namespace WindowsTerminalQuake
{
	public class Program
	{
		private static Toggler _toggler;
		private static TrayIcon _trayIcon;

		public static void Main(string[] args)
		{
			Logging.Configure();

			_trayIcon = new TrayIcon((s, a) => Close());

			try
			{
				var process = GetOrCreateWindowsTerminalProcess();
				process.EnableRaisingEvents = true;
				process.Exited += (sender, e) =>
				{
					Close();
				};
				_toggler = new Toggler(process);

				// Transparency
				Settings.Get(s =>
				{
					TransparentWindow.SetTransparent(process, s.Opacity);
				});

				var hks = string.Join(" or ", Settings.Instance.Hotkeys.Select(hk => $"{hk.Modifiers}+{hk.Key}"));

				_trayIcon.Notify(ToolTipIcon.Info, $"Windows Terminal Quake is running, press {hks} to toggle.");
			}
			catch (Exception ex)
			{
				Log.Logger.Warning($"T:ex: {ex.Message}\n{ex.StackTrace}");
				_trayIcon.Notify(ToolTipIcon.Error, $"Cannot start: '{ex.Message}'.");

				Close();
			}
		}

		private static Process GetOrCreateWindowsTerminalProcess()
		{
			var process = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();

			if (process == null)
			{
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
				
				if (process == null || process.HasExited)
				{
					throw new Exception("Can not ensure exited process is accessible");
				}

				try
				{
					// Note: Accessing mainWindowHandle already throws "Process has exited, so the requested information is not available."
					if (process.MainWindowHandle == IntPtr.Zero)
					{
						throw new Exception("Can not access newly started process.");
					}
				}
				catch (Exception)
				{
					process = Process.GetProcessesByName("WindowsTerminal").First();
					// _process.WaitForInputIdle();
					process.Refresh();
				}
			}

			return process;
		}

		private static void Close()
		{
			_toggler?.Dispose();
			_toggler = null;

			_trayIcon?.Dispose();
			_trayIcon = null;
		}
	}
}
