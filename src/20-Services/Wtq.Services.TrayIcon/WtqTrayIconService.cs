using Microsoft.Extensions.Hosting;
using NotificationIcon.NET;
using System.Runtime.InteropServices;

namespace Wtq.Services.TrayIcon;

public sealed class WtqTrayIconService : WtqHostedService
{
	private readonly bool _isWin = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

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
		// It seems Windows wants its icons as ICO files, while Linux supports PNG.
		// Also, on Linux the icon requires some padding.
		var iconPath = _isWin
			? WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-nopadding.ico")
			: WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-256-padding.png");

		_icon = NotifyIcon.Create(
			iconPath,
			[
				CreateItem(
					$"Version {WtqConstants.AppVersion}",
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

		if (_isWin)
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
			Click = (s, e) => action(),
			IsDisabled = !enabled,
		};
	}
}