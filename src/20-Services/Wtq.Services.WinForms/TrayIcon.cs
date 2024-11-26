using Wtq.Configuration;

namespace Wtq.Services.WinForms;

public sealed class TrayIcon : IDisposable
{
	private NotifyIcon? _notificationIcon;

	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "MvdO: Replace with simple tray icon?")]
	public TrayIcon(
		IHostApplicationLifetime lifetime,
		IWtqBus bus)
	{
		_ = Guard.Against.Null(lifetime);
		_ = Guard.Against.Null(bus);

		var waiter = new TaskCompletionSource<bool>();

		var notifyThread = new Thread(() =>
		{
			var contextMenu = new ContextMenuStrip();

			contextMenu.Items.AddRange(
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion}",
					() => {},
					enabled: false),

				new ToolStripSeparator(),

				CreateItem(
					"Open Project Website (GitHub)",
					() => Os.OpenUrl(WtqConstants.GitHubUrl)),

				new ToolStripSeparator(),

				CreateItem(
					"Open Main Window",
					() => bus.Publish(new WtqUIRequestedEvent())),

				new ToolStripSeparator(),

				CreateItem(
					"Open Settings File",
					() => Os.OpenFileOrDirectory(WtqOptionsPath.Instance.Path)),

				CreateItem(
					"Open Settings Directory",
					() => Os.OpenFileOrDirectory(Path.GetDirectoryName(WtqOptionsPath.Instance.Path)!)),

				CreateItem(
					"Open Logs Directory",
					() => Os.OpenFileOrDirectory(WtqPaths.GetWtqLogDir())),

				new ToolStripSeparator(),

				CreateItem(
					"Exit",
					() => lifetime.StopApplication()),
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

	private static Icon CreateIcon()
	{
		using var str = new MemoryStream(Resources.Resources.icon_v2_64);

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