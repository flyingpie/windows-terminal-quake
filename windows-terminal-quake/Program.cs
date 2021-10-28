using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.Settings;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake
{
	public class Program
	{
		private static Toggler? _toggler;
		private static TrayIcon? _trayIcon;

		public static string GetVersion()
		{
			try
			{
				var ass = typeof(Program).Assembly;
				var ver = FileVersionInfo.GetVersionInfo(ass.Location);

				return ver.FileVersion;
			}
			catch (Exception ex)
			{
				Log.Error(ex, $"Could not get app version: {ex.Message}");
				return "(could not get version)";
			}
		}

		public static void Main(string[] args)
		{
			Logging.Configure();

			Log.Information("Windows Terminal Quake started");

			_trayIcon = new TrayIcon((s, a) => Close());

			try
			{
				TerminalProcess.OnExit(() => Close());

				_toggler = new Toggler(args);

				// Transparency
				QSettings.Get(s => TerminalProcess.Get(args).SetTransparency(s.Opacity));

				var hotkeys = string.Join(" or ", QSettings.Instance.Hotkeys.Select(hk => $"{hk.Modifiers}+{hk.Key}"));

				_trayIcon.Notify(ToolTipIcon.Info, $"Windows Terminal Quake is running, press {hotkeys} to toggle.");
			}
			catch (Exception ex)
			{
				Log.Logger.Warning(ex, $"Error: {ex.Message}");

				MessageBox.Show($"Error starting Windows Terminal Quake: {ex.Message}", "Ah nej :(");

				Close();
			}
		}

		private static void Close()
		{
			_toggler?.Dispose();
			_trayIcon?.Dispose();
		}
	}
}