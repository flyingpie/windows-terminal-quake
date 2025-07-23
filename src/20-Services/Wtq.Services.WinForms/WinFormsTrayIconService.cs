namespace Wtq.Services.WinForms;

internal sealed class WinFormsTrayIconService(
	IHostApplicationLifetime lifetime,
	IPlatformService platform,
	IWtqBus bus)
	: WtqHostedService
{
	private readonly IHostApplicationLifetime _lifetime = Guard.Against.Null(lifetime);
	private readonly IPlatformService _platform = Guard.Against.Null(platform);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private TrayIcon? _icon;

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_icon = new TrayIcon(_lifetime, _platform, _bus);

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_icon?.Dispose();

		return ValueTask.CompletedTask;
	}
}