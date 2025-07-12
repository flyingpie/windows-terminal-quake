using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqTrayIconService>();

	private readonly IHostApplicationLifetime _lifetime;
	private readonly IPlatformService _platform;
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private NotifyIcon? _icon;
	private Worker? _loop;

	public WtqTrayIconService(
		IHostApplicationLifetime lifetime,
		IPlatformService platform,
		IWtqBus bus,
		IWtqUIService ui)
	{
		_lifetime = lifetime;
		_bus = Guard.Against.Null(bus);
		_platform = Guard.Against.Null(platform);
		_ui = Guard.Against.Null(ui);

		new Thread(ShowStatusIcon).Start();
	}

	protected override async ValueTask OnDisposeAsync()
	{
		if (_icon != null)
		{
			_icon.Dispose();
			_icon = null;
		}

		if (_loop != null)
		{
			await _loop.DisposeAsync().NoCtx();
			_loop = null;
		}
	}

	private void ShowStatusIcon()
	{
		_icon = NotifyIcon.Create(
			_platform.PathToTrayIcon,
			[
				#pragma warning disable SA1025
				#pragma warning disable SA1027

				// @formatter:off
				CreateItem(
					$"Version {WtqConstants.AppVersion} ({_platform.PlatformName})",
					() => { },
					enabled: false),

				new SeparatorItem(),

				CreateItem(
					$"Open Project Website (GitHub)",
					() => _platform.OpenUrl(WtqConstants.GitHubUrl)),

				new SeparatorItem(),

				CreateItem(
					"Open Main Window",
					() => _bus.Publish(new WtqUIRequestedEvent())),

				new SeparatorItem(),

				CreateItem(
					"Open Settings File",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConf)),

				CreateItem(
					"Open Settings Directory",
					() => _platform.OpenFileOrDirectory(_platform.PathToWtqConfDir)),

				CreateItem(
					"Open Logs",
					() => _platform.OpenFileOrDirectory(_platform.PathToLogs)),

				new SeparatorItem(),

				CreateItem(
					"Quit",
					() => _lifetime.StopApplication()),

				// @formatter:on
				#pragma warning restore SA1025
				#pragma warning restore SA1027
			]);

		if (/*Os.IsWindows*/false) // TODO
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
			_loop = new(
				nameof(WtqTrayIconService),
				ct =>
				{
					_ui.RunOnUIThread(() => _icon.MessageLoopIteration(true));

					return Task.CompletedTask;
				},
				TimeSpan.FromMilliseconds(200));
		}
	}

	private static MenuItem CreateItem(
		string text,
		Action action,
		bool enabled = true)
	{
		return new MenuItem(text)
		{
			Click = (_, _) => action(), IsDisabled = !enabled,
		};
	}

	// private string GetPathToIcon()
	// {
	// 	// It seems Windows wants its icons as ICO files, while Linux supports PNG.
	// 	if (Os.IsWindows)
	// 	{
	// 		_log.LogDebug("Running on Windows, using ICO version of tray icon");
	// 		return WtqPaths.GetPathRelativeToWtqAppDir("assets", "nl.flyingpie.wtq-white.ico");
	// 	}
	//
	// 	// Linux (Flatpak).
	// 	if (Os.IsFlatpak)
	// 	{
	// 		_log.LogDebug("Running in Flatpak, using icon name of tray icon (i.e., not the full path)");
	// 		return "nl.flyingpie.wtq-white";
	// 	}
	//
	// 	// Linux (non-Flatpak).
	// 	_log.LogDebug("Running bare Linux, using icon path of tray icon");
	// 	return WtqPaths.GetPathRelativeToWtqAppDir("assets", "nl.flyingpie.wtq-white.svg");
	// }
}