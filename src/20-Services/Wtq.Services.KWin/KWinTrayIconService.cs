using Microsoft.Extensions.Hosting;
using NotificationIcon.NET;
using Wtq.Configuration;
using Wtq.Events;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IAsyncDisposable, IHostedService
{
	private readonly IHostApplicationLifetime _lifetime;
	private readonly IWtqBus _bus;
	private readonly IWtqUIService _ui;

	private Thread? _iconThread;
	private Worker? _loop;
	private NotifyIcon? _icon;

	public KWinTrayIconService(
		IHostApplicationLifetime lifetime,
		IWtqBus bus,
		IWtqUIService ui)
	{
		_lifetime = lifetime;
		_bus = Guard.Against.Null(bus);
		_ui = Guard.Against.Null(ui);

		_iconThread = new Thread(ShowStatusIcon);
		_iconThread.Start();
	}

	public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public async ValueTask DisposeAsync()
	{
		await (_loop?.DisposeAsync() ?? ValueTask.CompletedTask).NoCtx();
	}

	private void ShowStatusIcon()
	{
		var iconPath = WtqPaths.GetPathRelativeToWtqAppDir("assets", "icon-v2-64.png");

		_icon = NotifyIcon.Create(
			iconPath,
			[
				CreateVersionItem(),

				new SeparatorItem(),

				CreateOpenWebsiteItem(),

				CreateOpenSettingsItem(),

				CreateOpenSettingsFileItem(),

				CreateOpenSettingsDirItem(),

				CreateOpenLogItem(),

				new SeparatorItem(),

				CreateExitItem(),
			]);

		_loop = new(
			nameof(KWinTrayIconService),
			ct =>
			{
				_ui.RunOnUIThread(() => { _icon.MessageLoopIteration(true); });

				return Task.CompletedTask;
			},
			TimeSpan.FromMilliseconds(100));
	}

	private static MenuItem CreateOpenSettingsDirItem()
	{
		var mnuOpenSettings = new MenuItem("Open settings directory")
		{
			Click = (s, a) =>
			{
				Process.Start(
					new ProcessStartInfo()
					{
						FileName = Path.GetDirectoryName(WtqOptionsPath.Instance.Path), UseShellExecute = true,
					});
			},
		};

		return mnuOpenSettings;
	}

	private static MenuItem CreateOpenLogItem()
	{
		var mnuOpenSettings = new MenuItem("Open logs")
		{
			Click = (s, a) =>
			{
				Process.Start(
					new ProcessStartInfo()
					{
						FileName = WtqPaths.GetWtqLogDir(), UseShellExecute = true,
					});
			},
		};

		return mnuOpenSettings;
	}

	private static MenuItem CreateVersionItem()
	{
		var ver = typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

		return new MenuItem($"Version {ver}")
		{
			IsDisabled = true,
		};
	}

	private static MenuItem CreateOpenWebsiteItem()
	{
		var item = new MenuItem($"Open project website (GitHub)")
		{
			Click = (s, a) => Os.OpenUrl(WtqConstants.GitHubUrl),
		};

		return item;
	}

	private static MenuItem CreateOpenSettingsFileItem()
	{
		var mnuOpenSettings = new MenuItem("Open settings file")
		{
			Click = (s, a) =>
			{
				Process.Start(
					new ProcessStartInfo()
					{
						FileName = WtqOptionsPath.Instance.Path, UseShellExecute = true,
					});
			},
		};

		return mnuOpenSettings;
	}

	private MenuItem CreateOpenSettingsItem()
	{
		return new MenuItem("Open settings")
		{
			Click = (s, e) => _bus.Publish(new WtqUIRequestedEvent()),
		};
	}

	private MenuItem CreateExitItem()
	{
		return new MenuItem("Quit")
		{
			Click = (s, e) => _lifetime.StopApplication(),
		};
	}
}