using Microsoft.Extensions.Hosting;

namespace Wtq.Services.WinForms;

internal sealed class WinFormsTrayIconService : IDisposable, IHostedService
{
	private readonly IHostApplicationLifetime _lifetime;
	private TrayIcon? _icon;

	public WinFormsTrayIconService(IHostApplicationLifetime lifetime)
	{
		_lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
	}

	public void Dispose()
	{
		_icon?.Dispose();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_icon = new TrayIcon((s, a) => _lifetime.StopApplication());

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_icon?.Dispose();

		return Task.CompletedTask;
	}
}