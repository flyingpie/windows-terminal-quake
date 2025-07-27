namespace Wtq.Services.WinForms;

public sealed class TrayIcon : IDisposable
{
	private readonly IPlatformService _platform;
	private readonly TrayIconUtil _trayIconUtil;

	private NotifyIcon? _notificationIcon;

	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "MvdO: Replace with simple tray icon?")]
	public TrayIcon(
		IHostApplicationLifetime lifetime,
		IWtqBus bus,
		IPlatformService platform,
		TrayIconUtil trayIconUtil)
	{
		_ = Guard.Against.Null(bus);
		_ = Guard.Against.Null(lifetime);
		_platform = Guard.Against.Null(platform);
		_trayIconUtil = Guard.Against.Null(trayIconUtil);

		var waiter = new TaskCompletionSource<bool>();

		var notifyThread = new Thread(() =>
		{
			var contextMenu = new ContextMenuStrip();

			contextMenu.Items.AddRange(
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion}",
					() => { },
					enabled: false),

				new ToolStripSeparator(),

				CreateItem(
					"Open Project Website (GitHub)",
					() => _platform.OpenUrl(WtqConstants.GitHubUrl)),

				new ToolStripSeparator(),

				CreateItem(
					"Open Main Window",
					() => bus.Publish(new WtqUIRequestedEvent())),

				new ToolStripSeparator(),

				CreateItem(
					"Open Settings File",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConf)),

				CreateItem(
					"Open Settings Directory",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConfDir)),

				CreateItem(
					"Open Logs Directory",
					() => _platform.OpenFileOrDirectory(_platform.PathToLogsDir)),

				new ToolStripSeparator(),

				CreateItem(
					"Exit",
					() => lifetime.StopApplication()),
			]);

			// Tray Icon
			_notificationIcon = new NotifyIcon()
			{
				Icon = CreateIcon(), ContextMenuStrip = contextMenu, Text = "WTQ", Visible = true,
			};

			waiter.SetResult(true);

			Application.Run();
		})
		{
			Name = nameof(WinFormsTrayIconService),
		};

		notifyThread.Start();

		waiter.Task.GetAwaiter().GetResult();
	}

	public void Dispose()
	{
		_notificationIcon?.Dispose();
		_notificationIcon = null;

		Application.Exit();
	}

	private Icon CreateIcon()
	{
		using var str = File.OpenRead(_trayIconUtil.TrayIconPath);

		return new Icon(str);
	}

	private static ToolStripMenuItem CreateItem(
		string text,
		Action action,
		bool enabled = true)
	{
		var mnuOpenSettings = new ToolStripMenuItem(text)
		{
			Enabled = enabled,
		};

		mnuOpenSettings.Click += (s, a) => action();

		return mnuOpenSettings;
	}
}