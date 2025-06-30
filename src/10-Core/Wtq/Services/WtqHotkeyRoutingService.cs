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

	private string? _prevAppName;

	public WtqHotkeyRoutingService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_opts = opts ?? throw new ArgumentNullException(nameof(opts));
		_appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_bus.OnEvent<WtqHotkeyPressedEvent>(e => HandleHotkeyPressedEventAsync(e.Sequence));
	}

	public Task HandleHotkeyPressedEventAsync(KeySequence sequence)
	{
		// Look for app that has the specified hotkey configured.
		var app = _opts.CurrentValue.Apps.FirstOrDefault(app => app.Hotkeys.Any(hk => hk.Sequence == sequence));
		if (app != null)
		{
			_bus.Publish(new WtqAppToggledEvent(app.Name));
			_prevAppName = app.Name;

			return Task.CompletedTask;
		}

		// Look for a global (non-app-specific) hotkey.
		if (_opts.CurrentValue.Hotkeys.Any(hk => hk.Sequence == sequence))
		{
			var globalApp = _prevAppName // Prefer most recently toggled app.
				?? _appRepo.GetPrimary()?.Name; // Fall back to first configured app after that.

			if (globalApp == null)
			{
				_log.LogWarning("Could not find a candidate app for global hotkey '{Sequence}', maybe no apps were configured?", sequence);
				return Task.CompletedTask;
			}

			_bus.Publish(new WtqAppToggledEvent(globalApp));
			_prevAppName = globalApp;

			return Task.CompletedTask;
		}

		_log.LogWarning("Got unmapped hotkey '{Sequence}'. This could mean something went wrong during hotkey registration", sequence);
		return Task.CompletedTask;
	}
}