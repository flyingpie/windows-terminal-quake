using Microsoft.Extensions.Hosting;
using NotificationIcon.NET;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService : WtqHostedService
{
	private readonly IHostApplicationLifetime _lifetime;
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private NotifyIcon? _icon;
	private Worker? _loop;

	public KWinTrayIconService(
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
		var iconPath = WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-64.png");

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

		_loop = new(
			nameof(KWinTrayIconService),
			ct =>
			{
				_ui.RunOnUIThread(() => _icon.MessageLoopIteration(true));

				return Task.CompletedTask;
			},
			TimeSpan.FromMilliseconds(200));
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