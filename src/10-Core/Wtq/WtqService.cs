using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.Apps;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	WtqAppMonitorService appMon,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: IHostedService
{
	private readonly WtqAppMonitorService _appMon = appMon ?? throw new ArgumentNullException(nameof(appMon));
	private readonly IWtqAppRepo _appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
	private readonly IWtqBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));
	private readonly ILogger<WtqService> _log = log ?? throw new ArgumentNullException(nameof(log));

	private WtqApp? _lastOpen;
	private WtqApp? open;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Starting");

		_bus.OnEvent<WtqToggleAppEvent>(HandleToggleAppEventAsync);
		_bus.OnEvent<WtqAppFocusEvent>(HandleAppFocusEventAsync);

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Stopping");

		return Task.CompletedTask;
	}

	private async Task HandleAppFocusEventAsync(WtqAppFocusEvent ev)
	{
		if (ev.App != null && ev.App == open && !ev.GainedFocus)
		{
			await open.CloseAsync().ConfigureAwait(false);
			open = null;
		}
	}

	private async Task HandleToggleAppEventAsync(WtqToggleAppEvent ev)
	{
		var app = ev.App;

		// If the action does not point to a single app, toggle the most recent one.
		if (app == null)
		{
			if (open != null)
			{
				await open.CloseAsync().ConfigureAwait(false);
				_lastOpen = open; // TODO: Doesn't include focus lost.
				open = null;
				_appMon.RefocusLastNonWtqApp();
				return;
			}
			else
			{
				if (_lastOpen == null)
				{
					// TODO
					var first = _appRepo.Apps.FirstOrDefault();
					if (first != null)
					{
						await first.OpenAsync().ConfigureAwait(false);
					}

					open = first;
					_lastOpen = first;
					return;
				}

				open = _lastOpen;
				await open.OpenAsync().ConfigureAwait(false);
				return;
			}
		}

		if (open != null)
		{
			if (open == app)
			{
				await app.CloseAsync().ConfigureAwait(false);
				_lastOpen = open;
				open = null;
				_appMon.RefocusLastNonWtqApp();
			}
			else
			{
				await open.CloseAsync(ToggleModifiers.SwitchingApps).ConfigureAwait(false);
				await app.OpenAsync(ToggleModifiers.SwitchingApps).ConfigureAwait(false);

				open = app;
			}

			return;
		}

		// Open the specified app.
		_log.LogInformation("Toggling app {App}", app);
		if (await app.OpenAsync().ConfigureAwait(false))
		{
			open = app;
		}
	}
}