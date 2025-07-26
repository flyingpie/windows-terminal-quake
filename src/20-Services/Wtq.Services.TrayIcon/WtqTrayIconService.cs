using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService : WtqHostedService
{
	private readonly IPlatformService _platform;
	private readonly ILogger _log = Log.For<WtqTrayIconService>();

	private readonly NotifyIcon _icon;
	private readonly RecurringTask _loop;

	public WtqTrayIconService(
		IHostApplicationLifetime lifetime,
		IPlatformService platform,
		IWtqBus bus,
		IWtqUIService ui)
	{
		_ = Guard.Against.Null(bus);
		_ = Guard.Against.Null(lifetime);
		_ = Guard.Against.Null(ui);
		_platform = Guard.Against.Null(platform);

		_icon = NotifyIcon.Create(
			_platform.PathToTrayIcon,
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion} ({_platform.PlatformName})",
					() => { },
					enabled: false),

				new SeparatorItem(),

				CreateItem(
					"Open Project Website (GitHub)",
					() => _platform.OpenUrl(WtqConstants.GitHubUrl)),

				new SeparatorItem(),

				CreateItem(
					"Open Main Window",
					() => bus.Publish(new WtqUIRequestedEvent())),

				new SeparatorItem(),

				CreateItem(
					"Open Settings File",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConf)),

				CreateItem(
					"Open Settings Directory",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConfDir)),

				CreateItem(
					"Open Logs",
					() => _platform.OpenFileOrDirectory(_platform.PathToLogsDir)),

				new SeparatorItem(),

				CreateItem(
					"Quit",
					lifetime.StopApplication),
			]);

		_loop = new(
			nameof(WtqTrayIconService),
			TimeSpan.FromMilliseconds(200), // The action is blocking, so this value doesn't actually matter much.
			ct =>
			{
				ui.RunOnUIThread(() => _icon.MessageLoopIteration(true));

				return Task.CompletedTask;
			});
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		new Thread(ShowStatusIcon).Start();

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_icon.Dispose();

		// Don't wait for the loop to fully dispose, as it can take a while due to the thread waiting on a GUI loop iteration.
		_ = _loop.DisposeAsync();

		return ValueTask.CompletedTask;
	}

	private void ShowStatusIcon()
	{
		if (Os.IsWindows)
		{
			// On Windows, we can just block the thread on the tray icon UI.
			_icon.Show();
		}
		else
		{
			// But on Linux, the main UI uses webkitgtk, which is also used by NotificationIcon.NET.
			// That means the threads can step on each other's state, crashing the UI stack.
			//
			// So we send any UI work to the main UI's thread, keeping UI stuff single-threaded.
			_loop.Start();
		}
	}

	private static MenuItem CreateItem(
		string text,
		Action action,
		bool enabled = true) =>
		new(text)
		{
			Click = (_, _) => action(),
			IsDisabled = !enabled,
		};
}