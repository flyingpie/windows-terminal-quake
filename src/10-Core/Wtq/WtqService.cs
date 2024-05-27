using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.Apps;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	IOptionsMonitor<WtqOptions> opts,
	WtqAppMonitorService appMon,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: IHostedService
{
	private readonly WtqAppMonitorService _appMon = Guard.Against.Null(appMon);
	private readonly IWtqAppRepo _appRepo = Guard.Against.Null(appRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly ILogger<WtqService> _log = Guard.Against.Null(log);
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly SemaphoreSlim _lock = new(1);

	private WtqApp? _lastOpen;
	private WtqApp? _open;

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
		return;
		// If focus moved to a different window, toggle out the current one (if there is an active app, and it's configured as such).
		if (ev.App != null &&
			ev.App == _open &&
			!ev.GainedFocus &&
			_opts.CurrentValue.GetHideOnFocusLostForApp(ev.App.Options))
		{
			await _open.CloseAsync().ConfigureAwait(false);
			_lastOpen = _open;
			_open = null;
		}
	}

	private async Task HandleToggleAppEventAsync(WtqToggleAppEvent ev)
	{
		try
		{
			await _lock.WaitAsync().ConfigureAwait(false);

			var app = ev.App;

			// If the action does not point to a single app, toggle the most recent one.
			if (app == null)
			{
				// If we still have an app open, close it now.
				if (_open != null)
				{
					await _open.CloseAsync().ConfigureAwait(false);
					_lastOpen = _open;
					_open = null;
					await _appMon.RefocusLastNonWtqAppAsync();
					return;
				}

				// If we don't yet have an app open, open either the most recently used one, or the first one.
				if (_lastOpen == null)
				{
					// TODO
					var first = _appRepo.Apps.FirstOrDefault();
					if (first != null)
					{
						await first.OpenAsync().ConfigureAwait(false);
					}

					_open = first;
					_lastOpen = first;
					return;
				}

				_open = _lastOpen;
				await _open.OpenAsync().ConfigureAwait(false);
				return;
			}

			if (_open != null)
			{
				if (_open == app)
				{
					await app.CloseAsync().ConfigureAwait(false);
					_lastOpen = _open;
					_open = null;
					await _appMon.RefocusLastNonWtqAppAsync();
				}
				else
				{
					await _open.CloseAsync(ToggleModifiers.SwitchingApps).ConfigureAwait(false);
					await app.OpenAsync(ToggleModifiers.SwitchingApps).ConfigureAwait(false);

					_open = app;
				}

				return;
			}

			// Open the specified app.
			_log.LogInformation("Toggling app {App}", app);
			if (await app.OpenAsync().ConfigureAwait(false))
			{
				_open = app;
			}
		}
		finally
		{
			_lock.Release();
		}
	}
}