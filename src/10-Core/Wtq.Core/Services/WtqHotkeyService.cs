using Microsoft.Extensions.Hosting;
using System.Threading;
using Wtq.Core.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Events;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq.Core.Service;

public class WtqHotkeyService : IHostedService, IWtqHotkeyService
{
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;

	public WtqHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_opts = opts ?? throw new ArgumentNullException(nameof(opts));
		_appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_opts.OnChange(SendRegisterEvents);

		_bus.On(
			e => e is WtqHotkeyPressedEvent,
			e =>
			{
				var x = (WtqHotkeyPressedEvent)e;

				_bus.Publish(new WtqToggleAppEvent()
				{
					ActionType = WtqActionType.ToggleApp,
					App = GetAppForHotkey(x.Modifiers, x.Key),
				});

				return Task.CompletedTask;
			});
	}

	private void SendRegisterEvents(WtqOptions opts)
	{
		// _log
		foreach (var app in opts.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				_bus.Publish(new WtqRegisterHotkeyEvent()
				{
					Key = hk.Key,
					Modifiers = hk.Modifiers,
				});
			}
		}

		foreach (var hk in opts.Hotkeys)
		{
			_bus.Publish(new WtqRegisterHotkeyEvent()
			{
				Key = hk.Key,
				Modifiers = hk.Modifiers,
			});
		}
	}

	public WtqApp? GetAppForHotkey(WtqKeyModifiers keyMods, WtqKeys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotkey(key, keyMods));
		if (opt == null)
		{
			return null;
		}

		var res = _appRepo.GetProcessForApp(opt);

		return res;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		SendRegisterEvents(_opts.CurrentValue);

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}