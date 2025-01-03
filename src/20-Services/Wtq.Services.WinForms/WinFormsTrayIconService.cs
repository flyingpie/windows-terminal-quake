namespace Wtq.Services.WinForms;

internal sealed class WinFormsTrayIconService(
	IHostApplicationLifetime lifetime,
	IWtqBus bus)
	: WtqHostedService
{
	private readonly IHostApplicationLifetime _lifetime = Guard.Against.Null(lifetime);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private TrayIcon? _icon;

	protected override ValueTask OnDisposeAsync()
	{
		_icon?.Dispose();

		return base.OnDisposeAsync();
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_icon = new TrayIcon(_lifetime, _bus);

		return Task.CompletedTask;
	}
}