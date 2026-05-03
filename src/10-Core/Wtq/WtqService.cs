using Wtq.Services;

namespace Wtq;

/// <summary>
/// Orchestrates toggling on- and off of apps, and sending focus to the right window.
/// </summary>
// TODO: Better name.
// TODO: Class desperately needs unit tests.
public sealed class WtqService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqService>();

	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly WtqSemaphoreSlim _lock = new(1, 1);

	public WtqService(
		IWtqAppRepo appRepo,
		IWtqBus bus)
	{
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
		// If previously toggled apps (that are not the to-be-toggled app) are still open, close them first.
		var isSwitching = false;
		foreach (var open in _appRepo.GetOpen().Where(o => o != app).ToList())
		{
			// If either this app, or the active one, wants to be the _only_ active one, close the open one.
			if (app.Options.GetExclusive() != Exclusive.Always && open.Options.GetExclusive() != Exclusive.Always)
			{
				continue;
			}

			_log.LogInformation("Closing app '{AppClosing}', opening app '{AppOpening}'", open, app);

			_bus.Publish(new WtqAppToggledOffEvent(open.Name, true));

			await open.CloseAsync(ToggleModifiers.SwitchingApps).NoCtx();

			isSwitching = true;
		}

		if (isSwitching)
		{
			_bus.Publish(new WtqAppToggledOnEvent(app.Name, true));

			await app.OpenAsync(ToggleModifiers.SwitchingApps).NoCtx();
			return;
		}

		// Move to current virtual desktop
		if (app.IsOpen && !(await app.Window.IsOnCurrentVirtualDesktopAsync()))
		{
			// TODO: Event
			_log.LogInformation("Moving app '{App}' to current virtual desktop", app);
			app.Window.SetTaskbarIconVisibleAsync(false);
			app.Window.SetTaskbarIconVisibleAsync(true);
		}

		// Re-focus
		if (app.IsOpen && !(await app.Window.HasFocusAsync()))
		{
			// TODO: Event
			_log.LogInformation("Re-focusing app '{App}'", app);
			await app.Window.BringToForegroundAsync();
			return;
		}

		// Toggling app OFF
		if (app.IsOpen)
		{
			// Toggle OFF
			_log.LogInformation("Closing previously open app '{App}'", app);

			_bus.Publish(new WtqAppToggledOffEvent(app.Name, false));

			// Close app.
			await app.CloseAsync().NoCtx();

			// Bring focus back to last non-WTQ app.
			await PopFocusAsync().NoCtx();
		}

		// Toggling app ON
		else
		{
			// Toggle ON
			_log.LogInformation("Opening previously closed app '{App}'", app);

			_bus.Publish(new WtqAppToggledOnEvent(app.Name, false));

			// Open app.
			await app.OpenAsync().NoCtx();
		}
	}

	private readonly List<(WtqWindow Window, WtqApp? App)> _lastNonWtqWindow = new(10);

	/// <summary>
	/// After closing an app, we bring focus back to the last window that had focus before toggling on the WTQ-managed one.<br/>
	/// This is so that user input gets sent to the last active window, instead of to the app we just toggled off, which is not off-screen.
	/// </summary>
	private async Task PopFocusAsync()
	{
		_log.LogDebug("Bringing back focus to the previously active window");

		while (true)
		{
			// Pull most recently added window from the "stack".
			var prev = _lastNonWtqWindow.LastOrDefault();
			_lastNonWtqWindow.Remove(prev);

			// Skip windows that are managed by WTQ and currently toggled off.
			if (prev.App is { IsOpen: false })
			{
				_log.LogDebug("Skipping window '{Window}', as it is being handled by WTQ, and is toggled OFF", prev.Window);
				continue;
			}

			// Bring focus to the window.
			_log.LogInformation("Bringing back focus to previously-active window '{Window}'", prev.Window);
			await prev.Window.BringToForegroundAsync().NoCtx();
			return;
		}
	}

	/// <summary>
	/// Handles events where the focus moved to another window.
	/// TODO: With the addition of multiple apps active, this needs some rework.
	/// </summary>
	private async Task OnWindowFocusChangedEventAsync(WtqWindowFocusChangedEvent ev)
	{
		Guard.Against.Null(ev);

		// Wait for service-wide lock.
		using var l = await _lock.WaitOneSecondAsync().NoCtx();

		// Look for apps that are attached to the windows that got- and lost focus, respectively.
		var appGotFocus = ev.GotFocusWindow != null ? _appRepo.GetByWindow(ev.GotFocusWindow) : null;
		var appLostFocus = ev.LostFocusWindow != null ? _appRepo.GetByWindow(ev.LostFocusWindow) : null;

		// Store the app that lost focus in our stack.
		if (ev.LostFocusWindow != null)
		{
			_lastNonWtqWindow.RemoveAll(w => w.Window.Id.Equals(ev.LostFocusWindow.Id, StringComparison.OrdinalIgnoreCase)); // Remove any previous instance.
			_lastNonWtqWindow.Add((ev.LostFocusWindow, appLostFocus));
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
			if (appLostFocus.Options.GetHideOnFocusLost() == HideOnFocusLost.Never)
			{
				return;
			}

			_log.LogInformation("App '{App}' lost focus, closing", appLostFocus);
			await appLostFocus.CloseAsync().NoCtx();
		}
	}
}