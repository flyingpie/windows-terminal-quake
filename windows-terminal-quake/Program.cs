using System;
using System.ComponentModel;
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
				Process process = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
				if (process == null)
				{
					process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = "wt",
							UseShellExecute = false,
							RedirectStandardOutput = true,
							WindowStyle = ProcessWindowStyle.Maximized,
							CreateNoWindow = true,
						}
					};
					process.Start();
					process = Process.GetProcessesByName("WindowsTerminal").First();
					process.WaitForInputIdle();
					process.Refresh();
					Log.Logger.Information("process was started from quake console (to be tested values)");
					LogProcessInformation(process);
				}

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

		private static void Close()
		{
			_toggler?.Dispose();
			_toggler = null;

			_trayIcon?.Dispose();
			_trayIcon = null;
		}

		private static void LogProcessInformation(Process process)
		{
			foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(process))
			{
				string name = descriptor.Name;
				if (new[] {"ExitCode", "ExitTime", "BasePriority", "StandardInput", "StandardOutput", "StandardError"}.Any(x =>
					x == name))
				{
					continue;
				}

				object value = descriptor.GetValue(process);

				Log.Logger.Information("Process: {0}={1}", name, value);
			}
		}
	}
}
