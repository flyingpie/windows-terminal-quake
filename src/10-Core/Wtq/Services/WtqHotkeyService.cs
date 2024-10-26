using Microsoft.Extensions.Hosting;
using Wtq.Events;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqHotKeyService"/>
public class WtqHotKeyService
	: IHostedService, IWtqHotKeyService
{
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	private WtqApp? _prevApp;

	public WtqHotKeyService(
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
				_bus.Publish(
					new WtqAppToggledEvent()
					{
						App = GetAppForHotKey(e.Modifiers, e.Key) ?? _prevApp ?? _appRepo.GetPrimary(),
					});

				return Task.CompletedTask;
			});
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

	private WtqApp? GetAppForHotKey(KeyModifiers keyMods, Keys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotKey(key, keyMods));

		return opt == null
			? null
			: _appRepo.GetByName(opt.Name);
	}

	private void SendRegisterEvents(WtqOptions opts)
	{
		// _log
		foreach (var app in opts.Apps)
		{
			foreach (var hk in app.HotKeys)
			{
				_bus.Publish(
					new WtqHotkeyDefinedEvent()
					{
						Key = hk.Key,
						Modifiers = hk.Modifiers,
					});
			}
		}

		foreach (var hk in opts.HotKeys)
		{
			_bus.Publish(
				new WtqHotkeyDefinedEvent()
				{
					Key = hk.Key,
					Modifiers = hk.Modifiers,
				});
		}
	}
}