namespace Wtq.Services;

/// <summary>
/// Receives raw hotkey events from a platform-specific service, and converts them to more
/// specific events, such as <see cref="WtqAppToggledEvent"/>.
/// </summary>
public class WtqHotkeyRoutingService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqHotkeyRoutingService>();

	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;

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

		_bus.OnEvent<WtqHotkeyPressedEvent>(e =>
		{
			// Look for app that has the specified hotkey configured.
			// Fall back to most recently toggled app.
			// Fall back to first configured app after that.
			var app = GetAppForHotkey(e.Sequence) ?? _prevApp ?? _appRepo.GetPrimary();

			if (app == null)
			{
				_log.LogWarning("No app found for hotkey '{Sequence}'", e.Sequence);
				return Task.CompletedTask;
			}

			_bus.Publish(new WtqAppToggledEvent(app.Name));

			_prevApp = app;

			return Task.CompletedTask;
		});
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		SendRegisterEvents(_opts.CurrentValue);

		return Task.CompletedTask;
	}

	private WtqApp? GetAppForHotkey(KeySequence sequence)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.Hotkeys.HasHotkey(sequence));

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
						AppOptions = app,
						Sequence = hk.Sequence,
					});
			}
		}

		foreach (var hk in opts.Hotkeys)
		{
			_bus.Publish(
				new WtqHotkeyDefinedEvent()
				{
					Sequence = hk.Sequence,
				});
		}
	}
}