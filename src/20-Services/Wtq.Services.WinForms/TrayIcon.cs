using Wtq.Configuration;

namespace Wtq.Services.WinForms;

public sealed class TrayIcon : IDisposable
{
	private readonly IHostApplicationLifetime _lifetime;
	private readonly IWtqBus _bus;

	private NotifyIcon? _notificationIcon;

	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "MvdO: Replace with simple tray icon?")]
	public TrayIcon(
		IHostApplicationLifetime lifetime,
		IWtqBus bus)
	{
		_lifetime = Guard.Against.Null(lifetime);
		_bus = Guard.Against.Null(bus);

		var waiter = new TaskCompletionSource<bool>();

		var notifyThread = new Thread(() =>
		{
			var contextMenu = new ContextMenuStrip();

			contextMenu.Items.AddRange(
			[
				CreateVersionItem(),

				new ToolStripSeparator(),

				CreateOpenWebsiteItem(),

				CreateOpenSettingsItem(),

				CreateOpenSettingsFileItem(),

				CreateOpenSettingsDirItem(),

				CreateOpenLogItem(),

				CreateExitItem(),
			]);

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

	public void Dispose()
	{
		_notificationIcon?.Dispose();
		_notificationIcon = null;
	}

	private static void OpenBrowser(Uri uri)
	{
		Guard.Against.Null(uri);

		Process.Start(new ProcessStartInfo(uri.ToString())
		{
			UseShellExecute = true,
		});
	}

	private ToolStripMenuItem CreateExitItem()
	{
		var mnuExit = new ToolStripMenuItem("Exit");

		mnuExit.Click += (s, a) => _lifetime.StopApplication();

		return mnuExit;
	}

	private static Icon CreateIcon()
	{
		using var str = new MemoryStream(Resources.Resources.icon);
		return new Icon(str);
	}

	private ToolStripMenuItem CreateOpenSettingsItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open settings")
		{
			Enabled = true,
		};

		mnuOpenSettings.Click += (s, a) =>
		{
			_bus.Publish(new WtqUIRequestedEvent());
		};

		return mnuOpenSettings;
	}

	private static ToolStripMenuItem CreateOpenSettingsFileItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open settings file")
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

	private static ToolStripMenuItem CreateOpenSettingsDirItem()
	{
		var mnuOpenSettings = new ToolStripMenuItem("Open settings directory")
		{
			Enabled = true,
		};

		mnuOpenSettings.Click += (s, a) =>
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = Path.GetDirectoryName(WtqOptionsPath.Instance.Path),
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