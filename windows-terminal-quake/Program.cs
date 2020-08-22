using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;

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
							WindowStyle = ProcessWindowStyle.Maximized
						}
					};
					process.Start();
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
	}
}