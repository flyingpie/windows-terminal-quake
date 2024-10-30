using Microsoft.Extensions.Hosting;
using Wtq.Events;

namespace Wtq.Services;

/// <summary>
/// Receives raw hotkey events from a platform-specific service, and converts them to more
/// specific events, such as <see cref="Events.WtqToggleAppEvent"/>.
/// </summary>
public class WtqHotkeyService : IHostedService, IWtqHotkeyService
{
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	public WtqHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_opts = opts ?? throw new ArgumentNullException(nameof(opts));
		_appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_opts.OnChange(SendRegisterEvents);

		_bus.OnEvent<WtqHotkeyPressedEvent>(
			e =>
			{
				_bus.Publish(new WtqToggleAppEvent()
				{
					App = GetAppForHotKey(e.Modifiers, e.Key),
				});

				return Task.CompletedTask;
			});
	}

	public WtqApp? GetAppForHotKey(KeyModifiers keyMods, Keys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotkey(key, keyMods));
		if (opt == null)
		{
			return null;
		}

		var res = _appRepo.GetAppByName(opt.Name);

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
}