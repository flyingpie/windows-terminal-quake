using Wtq.Services;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	IOptionsMonitor<WtqOptions> opts,
	IWtqAppRepo appRepo,
	IWtqBus bus,
	IWtqFocusTracker focusTracker)
	: IAsyncInitializable, IDisposable
{
	private readonly ILogger<WtqService> _log = Guard.Against.Null(log);
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqAppRepo _appRepo = Guard.Against.Null(appRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly IWtqFocusTracker _focusTracker = Guard.Against.Null(focusTracker);
	private readonly SemaphoreSlim _lock = new(1);

	private WtqApp? _lastOpen;
	private WtqApp? _open;
	private WtqWindow? _lastNonWtqWindow;

	public Task InitializeAsync()
	{
		_log.LogInformation("Starting");

		_bus.OnEvent<WtqAppToggledEvent>(HandleToggleAppEventAsync);

		_bus.OnEvent<WtqWindowFocusEvent>(HandleAppFocusEventAsync);

		return Task.CompletedTask;
	}

	public void Dispose()
	{
		_lock.Dispose();
	}

	private async Task HandleAppFocusEventAsync(WtqWindowFocusEvent ev)
	{
		Guard.Against.Null(ev);

		// Look for apps that are attached to the windows that got- and lost focus, respectively.
		var appGotFocus = ev.GotFocusWindow != null ? _appRepo.GetByWindow(ev.GotFocusWindow) : null;
		var appLostFocus = ev.LostFocusWindow != null ? _appRepo.GetByWindow(ev.LostFocusWindow) : null;

		if (appLostFocus != null)
		{
		}

		// If the window that just lost focus is not managed by WTQ, store it for giving back focus to later.
		if (ev.LostFocusWindow != null && appLostFocus == null)
		{
			_lastNonWtqWindow = ev.LostFocusWindow;
		}

		// // If focus moved to a different window, toggle out the current one (if there is an active app, and it's configured as such).
		// if (ev.App != null &&
		// 	ev.App == _open &&
		// 	!ev.GainedFocus &&
		// 	_opts.CurrentValue.GetHideOnFocusLostForApp(ev.App.Options))
		// {
		// 	await _open.CloseAsync().NoCtx();
		// 	_lastOpen = _open;
		// 	_open = null;
		// }
	}

	private async Task HandleToggleAppEventAsync(WtqAppToggledEvent ev)
	{
		try
		{
			await _lock.WaitAsync().NoCtx();

			// await OnAppToggleAsync(ev).NoCtx();
		}
		finally
		{
			_lock.Release();
		}
	}

	private async Task OnAppToggleAsync(WtqAppToggledEvent ev)
	{
		// Get the app for which a toggle event was fired (could be null).
		var app = ev.App;

		// If the event is not associated to a specific app, use either the most recently toggled one, or the primary one.
		app ??= _lastOpen ?? _appRepo.GetPrimary();

		// If the app is still null here, WTQ is probably not correctly configured.
		if (app == null)
		{
			_log.LogWarning("No app to toggle specified in the incoming event, no recently toggled app known, and no primary app available");
			return;
		}

		if (!app.IsOpen)
		{
			await app.OpenAsync().NoCtx();
		}

		// If we still have an app open, close it now.
		if (_open != null)
		{
			await _open.CloseAsync().NoCtx();
			_lastOpen = _open;
			_open = null;

			// await _focusTracker.FocusLastNonWtqAppAsync().NoCtx();
			return;
		}

		// // If the action does not point to a single app, toggle the most recent one.
		// if (app == null)
		// {
		// 	// If we don't yet have an app open, open either the most recently used one, or the first one.
		// 	if (_lastOpen == null)
		// 	{
		// 		// TODO
		// 		var first = _appRepo.GetPrimary();
		// 		if (first != null)
		// 		{
		// 			await first.OpenAsync().NoCtx();
		// 		}
		//
		// 		_open = first;
		// 		_lastOpen = first;
		// 		return;
		// 	}
		//
		// 	_open = _lastOpen;
		// 	await _open.OpenAsync().NoCtx();
		// 	return;
		// }

		if (_open != null)
		{
			if (_open == app)
			{
				await app.CloseAsync().NoCtx();
				_lastOpen = _open;
				_open = null;

				// await _focusTracker.FocusLastNonWtqAppAsync().NoCtx();
			}
			else
			{
				await _open.CloseAsync(ToggleModifiers.SwitchingApps).NoCtx();
				await app.OpenAsync(ToggleModifiers.SwitchingApps).NoCtx();

				_open = app;
			}

			return;
		}

		// Open the specified app.
		_log.LogInformation("Toggling app {App}", app);
		if (await app.OpenAsync().NoCtx())
		{
			_open = app;
		}
	}
}