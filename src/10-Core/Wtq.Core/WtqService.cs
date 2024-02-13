using Microsoft.Extensions.Hosting;
using System.Threading;
using Wtq.Core.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Events;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	IOptions<WtqOptions> opts,
	WtqAppMonitorService appMon,
	IWtqAppToggleService toggler,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: IHostedService
{
	private readonly ILogger<WtqService> _log = log ?? throw new ArgumentNullException(nameof(log));

	private readonly WtqAppMonitorService _appMon = appMon ?? throw new ArgumentNullException(nameof(appMon));
	private readonly IWtqAppRepo _appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
	private readonly IWtqBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Starting");

		_bus.OnAsync(e => e is WtqToggleAppEvent, ToggleStuffAsync);

		_bus.OnAsync(
			e => e is WtqAppFocusEvent,
			async e =>
			{
				var ev = (WtqAppFocusEvent)e;

				if (ev.App != null && ev.App == open && !ev.GainedFocus)
				{
					await open.CloseAsync();
					open = null;
				}
			});

		return Task.CompletedTask;
	}

	private WtqApp? open = null;
	private WtqApp? _lastOpen = null;

	private async Task ToggleStuffAsync(IWtqEvent ev)
	{
		var app = ev.App;

		// If the action does not point to a single app, toggle the most recent one.
		if (app == null)
		{
			if (open != null)
			{
				await open.CloseAsync();
				_lastOpen = open;
				open = null;
				_appMon.DropFocus();
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
						await first.OpenAsync();
					}
					open = first;
					_lastOpen = first;
					return;
				}

				open = _lastOpen;
				await open.OpenAsync();
				return;
			}

			return;
		}

		// We can't toggle apps that are not active.
		if (!app.IsActive)
		{
			_log.LogWarning("WTQ process for app '{App}' does not have a process instance assigned", app);
			return;
		}

		if (open != null)
		{
			if (open == app)
			{
				await app.CloseAsync();
				_lastOpen = open;
				open = null;
				_appMon.DropFocus();
			}
			else
			{
				await open.CloseAsync(ToggleModifiers.SwitchingApps);
				await app.OpenAsync(ToggleModifiers.SwitchingApps);

				open = app;
			}

			return;
		}

		_log.LogInformation("Toggling app {App}", app);
		await app.OpenAsync();

		open = app;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Stopping");

		return Task.CompletedTask;
	}
}