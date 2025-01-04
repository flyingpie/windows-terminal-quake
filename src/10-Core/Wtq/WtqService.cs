using Wtq.Services;

namespace Wtq;

/// <summary>
/// Orchestrates toggling on- and off of apps, and sending focus to the right window.
/// </summary>
// TODO: Better name.
public sealed class WtqService : WtqHostedService
{
	private readonly ILogger<WtqService> _log;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly WtqSemaphoreSlim _lock = new(1, 1);

	private WtqWindow? _lastNonWtqWindow;

	public WtqService(
		ILogger<WtqService> log,
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
		_log = Guard.Against.Null(log);
		_opts = Guard.Against.Null(opts);
		_appRepo = Guard.Against.Null(appRepo);
		_bus = Guard.Against.Null(bus);

		_bus.OnEvent<WtqAppToggledEvent>(OnAppToggledEventAsync);
		_bus.OnEvent<WtqWindowFocusChangedEvent>(OnWindowFocusChangedEventAsync);
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		// TODO: Currently necessary to make sure this service is constructed.
		// Maybe remove IHostedService and manually resolve this type on app startup?
		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_lock.Dispose();

		return ValueTask.CompletedTask;
	}

	/// <summary>
	/// Handles "toggle" events, e.g. where the user pressed a hotkey.
	/// </summary>
	private async Task OnAppToggledEventAsync(WtqAppToggledEvent ev)
	{
		var app = _appRepo.GetByName(ev.AppName);

		if (app == null)
		{
			_log.LogWarning("No app found with name '{AppName}'", ev.AppName);
			return;
		}

		// Wait for service-wide lock.
		using var l = await _lock.WaitOneSecondAsync().NoCtx();

		// "Switching apps"
		// If a previously toggled app (that is not the to-be-toggled app) is still open, close it first.
		var open = _appRepo.GetOpen();
		if (open != null && open != app)
		{
			_log.LogInformation("Closing app '{AppClosing}', opening app '{AppOpening}'", open, app);
			await open.CloseAsync(ToggleModifiers.SwitchingApps).NoCtx();
			await app.OpenAsync(ToggleModifiers.SwitchingApps).NoCtx();
			return;
		}

		// "Toggling app"
		if (app.IsOpen)
		{
			_log.LogInformation("Closing previously open app '{App}'", app);

			// Close app.
			await app.CloseAsync().NoCtx();

			// Bring focus back to last non-WTQ app.
			await (_lastNonWtqWindow?.BringToForegroundAsync() ?? Task.CompletedTask).NoCtx();
		}
		else
		{
			_log.LogInformation("Opening previously closed app '{App}'", app);

			// Open app.
			await app.OpenAsync().NoCtx();
		}
	}

	/// <summary>
	/// Handles events where the focus moved to another window.
	/// </summary>
	private async Task OnWindowFocusChangedEventAsync(WtqWindowFocusChangedEvent ev)
	{
		Guard.Against.Null(ev);

		// Wait for service-wide lock.
		using var l = await _lock.WaitOneSecondAsync().NoCtx();

		// Look for apps that are attached to the windows that got- and lost focus, respectively.
		var appGotFocus = ev.GotFocusWindow != null ? _appRepo.GetByWindow(ev.GotFocusWindow) : null;
		var appLostFocus = ev.LostFocusWindow != null ? _appRepo.GetByWindow(ev.LostFocusWindow) : null;

		// If the window that just LOST focus is NOT managed by WTQ, store it for giving back focus to later.
		if (ev.LostFocusWindow != null && appLostFocus == null)
		{
			_lastNonWtqWindow = ev.LostFocusWindow;
		}

		// If the app that GOT focus is a WTQ app, toggling will be done in the "app toggled" event handler.
		if (appGotFocus != null)
		{
			return;
		}

		// If the app that LOST focus is a WTQ app, toggle it off (depending on configuration).
		if (appLostFocus != null)
		{
			// If the app has "hide on focus lost" set to NEVER/FALSE, well, don't hide.
			if (_opts.CurrentValue.GetHideOnFocusLostForApp(appLostFocus.Options) == HideOnFocusLost.Never)
			{
				return;
			}

			_log.LogInformation("App '{App}' lost focus, closing", appLostFocus);
			await appLostFocus.CloseAsync().NoCtx();
		}
	}
}