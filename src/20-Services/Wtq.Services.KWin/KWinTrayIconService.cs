using Microsoft.Extensions.Hosting;
using NotificationIcon.NET;
using Wtq.Events;

namespace Wtq.Services.KWin;

public sealed class KWinTrayIconService
	: IDisposable, IHostedService
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

	public void Dispose()
	{
		_loop?.Dispose();
	}

	private void ShowStatusIcon()
	{
		var iconPath = Path.Join(AppContext.BaseDirectory, "icon.png");

		_icon = NotifyIcon.Create(
			iconPath,
			[
				CreateVersionItem(),

				new SeparatorItem(),

				CreateOpenSettingsItem(),

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

	private static MenuItem CreateVersionItem()
	{
		var ver = typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

		return new MenuItem($"Version {ver}")
		{
			IsDisabled = true,
		};
	}

	private MenuItem CreateOpenSettingsItem()
	{
		return new MenuItem("Open Settings")
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