using Wtq.Core;
using Wtq.Core.Resources;
using Wtq.Win32.Native;

namespace Wtq.WinForms;

public sealed class TrayIcon : IDisposable
{
	private NotifyIcon? _notificationIcon;

	public TrayIcon(Action<object, EventArgs> exitHandler)
	{
		var waiter = new TaskCompletionSource<bool>();

		var notifyThread = new Thread(() =>
		{
			var contextMenu = new ContextMenuStrip();

			contextMenu.Items.AddRange(new[]
			{
				// Version
				CreateVersionItem(),

				// Open settings file
				CreateOpenSettingsItem(),

				CreateConsoleItem(),

				// Exit
				CreateExitItem(exitHandler),
			});

			// Tray Icon
			_notificationIcon = new NotifyIcon()
			{
				Icon = CreateIcon(),
				ContextMenuStrip = contextMenu,
				Text = "Windows Terminal Quake",
				Visible = true,
			};

			waiter.SetResult(true);

			Application.Run();
		});

		notifyThread.Start();

		waiter.Task.GetAwaiter().GetResult();
	}

	private static ToolStripMenuItem CreateVersionItem()
	{
		return new ToolStripMenuItem($"Version v2.x.x")
		{
			Enabled = false,
		};
	}

	private static ToolStripMenuItem CreateOpenSettingsItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open settings")
		{
			Enabled = true,
		};

		mnuOpenSettings.Click += (s, a) =>
		{
			// TODO: We need to restore the original multi-location configuration file support.

			// var path = QSettings.Instance.PathToSettings;

			// if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
			// {
			// //Log.Warning($"Settings file '{path}' not found, attempting to create an example file now and opening that.");

			// //  Open the first default path.
			// path = QSettings.Instance.PathToSettings;

			// // Make sure it doesn't already exist, and only then write some stub JSON.
			// // "path" here may not be equal to "path" earlier, since we grabbed a path from PathToSettings.
			// if (!File.Exists(path))
			// {
			// //Log.Information($"Creating example file at '{path}'.");
			// File.WriteAllText(path, _Resources.windows_terminal_quake);
			// }
			// }
			Process.Start(new ProcessStartInfo()
			{
				FileName = App.PathToAppConf,
				UseShellExecute = true,
			});
		};

		return mnuOpenSettings;
	}

	private static ToolStripItem CreateConsoleItem()
	{
		var mnuExit = new ToolStripMenuItem("Pop Console");

		mnuExit.Click += new EventHandler((s, a) => Kernel32.AllocConsole());

		return mnuExit;
	}

	private static ToolStripItem CreateExitItem(Action<object, EventArgs> exitHandler)
	{
		var mnuExit = new ToolStripMenuItem("Exit");

		mnuExit.Click += new EventHandler(exitHandler);

		return mnuExit;
	}

	public void Dispose()
	{
		_notificationIcon?.Dispose();
		_notificationIcon = null;

		Application.Exit();
	}

	private static Icon CreateIcon()
	{
		using var str = new MemoryStream(Resources.icon);
		return new Icon(str);

		// var bitmap = Resources.icon.ToBitmap();
		// bitmap.MakeTransparent(Color.White);
		// var icH = bitmap.GetHicon();
		// var ico = Icon.FromHandle(icH);
		// return ico;
	}
}