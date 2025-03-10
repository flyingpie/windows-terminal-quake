namespace Wtq.Services;

/// <summary>
/// Receives raw hotkey events from a platform-specific service, and converts them to more
/// specific events, such as <see cref="WtqAppToggledEvent"/>.
/// </summary>
public class WtqHotkeyRoutingService : WtqHostedService//, IWtqHotkeyService
{
	private readonly ILogger _log = Log.For<WtqHotkeyRoutingService>();

	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	// private bool _isSuspended;

	private WtqApp? _prevApp;

	public WtqHotkeyRoutingService(
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
				// if (_isSuspended)
				// {
				// 	return Task.CompletedTask;
				// }

				// Look for app that has the specified hotkey configured.
				// Fall back to most recently toggled app.
				// Fall back to first configured app after that.
				var app = GetAppForHotkey(e.Modifiers, e.Key) ?? _prevApp ?? _appRepo.GetPrimary();

				if (app == null)
				{
					_log.LogWarning("No app found for hotkey '{Modifiers}+{Key}'", e.Modifiers, e.Key);
					return Task.CompletedTask;
				}

				_bus.Publish(
					new WtqAppToggledEvent()
					{
						AppName = app.Name,
					});

				_prevApp = app;

				return Task.CompletedTask;
			});
	}

	// public Task SuspendAsync(CancellationToken cancellationToken)
	// {
	// 	_isSuspended = true;
	//
	// 	_log.LogInformation("Suspending hotkey handling");
	//
	// 	return Task.CompletedTask;
	// }
	//
	// public Task ResumeAsync(CancellationToken cancellationToken)
	// {
	// 	_isSuspended = false;
	//
	// 	_log.LogInformation("Resuming hotkey handling");
	//
	// 	return Task.CompletedTask;
	// }

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		SendRegisterEvents(_opts.CurrentValue);

		return Task.CompletedTask;
	}

	private WtqApp? GetAppForHotkey(KeyModifiers keyMods, Keys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.Hotkeys.HasHotkey(key, keyMods));

		return opt == null
			? null
			: _appRepo.GetByName(opt.Name);
	}

	private void SendRegisterEvents(WtqOptions opts)
	{
		foreach (var app in opts.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				_bus.Publish(
					new WtqHotkeyDefinedEvent()
					{
						AppOptions = app, Key = hk.Key, Modifiers = hk.Modifiers,
					});
			}
		}

		foreach (var hk in opts.Hotkeys)
		{
			_bus.Publish(
				new WtqHotkeyDefinedEvent()
				{
					Key = hk.Key, Modifiers = hk.Modifiers,
				});
		}
	}
}