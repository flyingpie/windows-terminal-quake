namespace Wtq.Services.WinForms;

internal sealed class WinFormsTrayIconService(
	IHostApplicationLifetime lifetime,
	IPlatformService platform,
	IWtqBus bus,
	TrayIconUtil trayIconUtil
) : BackgroundService
{
	private readonly IHostApplicationLifetime _lifetime = Guard.Against.Null(lifetime);
	private readonly IPlatformService _platform = Guard.Against.Null(platform);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly TrayIconUtil _trayIconUtil = Guard.Against.Null(trayIconUtil);

	private TrayIcon? _icon;

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_icon = new TrayIcon(_lifetime, _bus, _platform, _trayIconUtil);

		return Task.CompletedTask;
	}

	public override void Dispose()
	{
		_icon?.Dispose();
	}

	/// <summary>
	/// Doesn't do anything after setting up tray icon.
	/// </summary>
	protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
		await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken).NoThrowAwaitable();
}
