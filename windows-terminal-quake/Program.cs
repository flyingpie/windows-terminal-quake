using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;
using WindowsTerminalQuake.Utils;

namespace WindowsTerminalQuake;

public static class Program
{
	private static readonly ILogger _log = Log.For(typeof(Program));

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
			_log.LogError(ex, "Could not get app version: {Message}", ex.Message);
			return "(could not get version)";
		}
	}

	public static void Main(string[] args)
	{
		Kernel32.AllocConsole();

		Console.WriteLine("Sup");

		_log.LogInformation("Windows Terminal Quake started");

		TerminalProcess.Init();

		_trayIcon = new TrayIcon((s, a) => Close());

		try
		{
			TerminalProcess.OnExit(() => Close());

			_toggler = new Toggler(args);

			// Transparency
			// TODO
			//QSettings.Get(s => TerminalProcess.Get(args).SetTransparency(s.Opacity));

			var hotkeys = string.Join(" or ", QSettings.Instance.Hotkeys.Select(hk => $"{hk.Modifiers}+{hk.Key}"));

			_trayIcon.Notify(ToolTipIcon.Info, $"Windows Terminal Quake is running, press {hotkeys} to toggle.");
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error: {Message}", ex.Message);

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