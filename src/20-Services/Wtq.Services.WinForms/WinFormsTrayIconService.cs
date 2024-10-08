using Microsoft.Extensions.Hosting;

namespace Wtq.Services.WinForms;

#pragma warning disable CA1812 // Avoid uninstantiated internal classes // MvdO: Instantiated through DI.
internal sealed class WinFormsTrayIconService : IDisposable, IHostedService
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
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
		_icon = new TrayIcon((s, a) =>
		{
			_lifetime.StopApplication();

			// TODO: Remove this, though currently not all threads exit properly.
			Application.Exit();
		});

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_icon?.Dispose();

		return Task.CompletedTask;
	}
}