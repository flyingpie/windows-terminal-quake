using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationIcon.NET;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqTrayIconService>();

	private readonly IHostApplicationLifetime _lifetime;
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private NotifyIcon? _icon;
	private Worker? _loop;

	public WtqTrayIconService(
		IHostApplicationLifetime lifetime,
		IWtqBus bus,
		IWtqUIService ui)
	{
		_lifetime = lifetime;
		_bus = Guard.Against.Null(bus);
		_ui = Guard.Against.Null(ui);

		new Thread(ShowStatusIcon).Start();
	}

	protected override ValueTask OnDisposeAsync()
	{
		if (_icon != null)
		{
			_icon.Dispose();
			_icon = null;
		}

		if (_loop != null)
		{
			// Don't wait for the loop to fully dispose, as it can take a while due to the thread waiting on a GUI loop iteration.
			_ = _loop.DisposeAsync();
		}

		return ValueTask.CompletedTask;
	}

	private void ShowStatusIcon()
	{
		_icon = NotifyIcon.Create(
			GetPathToIcon(),
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion}{(Os.IsFlatpak ? " (Flatpak)" : string.Empty)}",
					() => { },
					enabled: false),

				new SeparatorItem(),

				CreateItem(
					$"Open Project Website (GitHub)",
					() => Os.OpenUrl(WtqConstants.GitHubUrl)),

				new SeparatorItem(),

				CreateItem(
					"Open Main Window",
					() => _bus.Publish(new WtqUIRequestedEvent())),

				new SeparatorItem(),

				CreateItem(
					"Open Settings File",
					() => Os.OpenFileOrDirectory(WtqOptionsPath.Instance.Path)),

				CreateItem(
					"Open Settings Directory",
					() => Os.OpenFileOrDirectory(Path.GetDirectoryName(WtqOptionsPath.Instance.Path)!)),

				CreateItem(
					"Open Logs",
					() => Os.OpenFileOrDirectory(WtqPaths.GetWtqLogDir())),

				new SeparatorItem(),

				CreateItem(
					"Quit",
					() => _lifetime.StopApplication()),
			]);

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
			_loop = new(
				nameof(WtqTrayIconService),
				TimeSpan.FromMilliseconds(200),
				ct =>
				{
					if (!_loop.IsRunning)
					{
						return Task.CompletedTask;
					}

					_ui.RunOnUIThread(() => _icon.MessageLoopIteration(true));

					return Task.CompletedTask;
				});
		}
	}

	private static MenuItem CreateItem(
		string text,
		Action action,
		bool enabled = true)
	{
		return new MenuItem(text)
		{
			Click = (_, _) => action(),
			IsDisabled = !enabled,
		};
	}

	private string GetPathToIcon()
	{
		// It seems Windows wants its icons as ICO files, while Linux supports PNG.
		if (Os.IsWindows)
		{
			_log.LogDebug("Running on Windows, using ICO version of tray icon");
			return WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-nopadding.ico");
		}

		// Linux (Flatpak).
		if (Os.IsFlatpak)
		{
			_log.LogDebug("Running in Flatpak, using icon name of tray icon (i.e., not the full path)");
			return "nl.flyingpie.wtq-white";
		}

		// Linux (non-Flatpak).
		_log.LogDebug("Running bare Linux, using icon path of tray icon");
		return WtqPaths.GetPathRelativeToWtqAppDir("assets", "nl.flyingpie.wtq-white.svg");
	}
}