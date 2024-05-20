using Wtq.Configuration;

namespace Wtq.Services.WinForms;

public sealed class TrayIcon : IDisposable
{
	private NotifyIcon? _notificationIcon;

	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "MvdO: Replace with simple tray icon?")]
	public TrayIcon(Action<object?, EventArgs> exitHandler)
	{
		var waiter = new TaskCompletionSource<bool>();

		var notifyThread = new Thread(() =>
		{
			var contextMenu = new ContextMenuStrip();

			contextMenu.Items.AddRange(new ToolStripItem[]
			{
				CreateVersionItem(),

				new ToolStripSeparator(),

				CreateOpenWebsiteItem(),

				CreateOpenSettingsItem(),

				CreateOpenLogItem(),

				CreateExitItem(exitHandler),
			});

			// Tray Icon
			_notificationIcon = new NotifyIcon()
			{
				Icon = CreateIcon(),
				ContextMenuStrip = contextMenu,
				Text = "WTQ",
				Visible = true,
			};

			waiter.SetResult(true);

			Application.Run();
		});

		notifyThread.Start();

		waiter.Task.GetAwaiter().GetResult();
	}

	public static void OpenBrowser(Uri uri)
	{
		Guard.Against.Null(uri);

		Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
	}

	public void Dispose()
	{
		_notificationIcon?.Dispose();
		_notificationIcon = null;
	}

	private static ToolStripMenuItem CreateExitItem(Action<object?, EventArgs> exitHandler)
	{
		Guard.Against.Null(exitHandler);

		var mnuExit = new ToolStripMenuItem("Exit");

		mnuExit.Click += new EventHandler(exitHandler);

		return mnuExit;
	}

	private static Icon CreateIcon()
	{
		using var str = new MemoryStream(Resources.Resources.icon);
		return new Icon(str);
	}

	private static ToolStripMenuItem CreateOpenSettingsItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open settings")
		{
			Enabled = true,
		};

		mnuOpenSettings.Click += (s, a) =>
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = WtqOptionsPath.Instance.Path,
				UseShellExecute = true,
			});
		};

		return mnuOpenSettings;
	}

	private static ToolStripMenuItem CreateOpenLogItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open logs")
		{
			Enabled = true,
		};

		mnuOpenSettings.Click += (s, a) =>
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = WtqPaths.GetWtqLogDir(),
				UseShellExecute = true,
			});
		};

		return mnuOpenSettings;
	}

	private static ToolStripMenuItem CreateOpenWebsiteItem()
	{
		var item = new ToolStripMenuItem($"Open GitHub Project Website")
		{
			Enabled = true,
		};

		item.Click += (s, a) =>
		{
			OpenBrowser(WtqConstants.GitHubUrl);
		};

		return item;
	}

	private static ToolStripMenuItem CreateVersionItem()
	{
		var ver = typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

		return new ToolStripMenuItem($"Version {ver}")
		{
			Enabled = false,
		};
	}
}