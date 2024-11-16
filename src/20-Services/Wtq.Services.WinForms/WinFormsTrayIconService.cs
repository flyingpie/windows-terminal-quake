namespace Wtq.Services.WinForms;

internal sealed class WinFormsTrayIconService(
	IHostApplicationLifetime lifetime,
	IWtqBus bus)
	: IDisposable, IHostedService
{
	private readonly IHostApplicationLifetime _lifetime = Guard.Against.Null(lifetime);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private TrayIcon? _icon;

	public void Dispose()
	{
		_icon?.Dispose();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_icon = new TrayIcon(_lifetime, _bus);

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Dispose();

		return Task.CompletedTask;
	}
}