using Wtq.Events;
using Wtq.Services;

namespace Wtq;

/// <summary>
/// An "app" represents a single process that can be toggled (such as Windows Terminal).<br/>
/// It tracks its own state, and does not necessarily have a process attached.
/// </summary>

// TODO: Track whether the app is currently open, has focus, etc.
public sealed class WtqApp : IAsyncDisposable
{
	private readonly ILogger _log = Log.For<WtqApp>();

	private readonly Func<WtqAppOptions> _optionsAccessor;
	private readonly IWtqAppRepo _appRepo;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqWindowResolver _windowResolver;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqAppToggleService _toggler;

	public WtqApp(
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus,
		IWtqWindowResolver windowResolver,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqAppToggleService toggler,
		Func<WtqAppOptions> optionsAccessor,
		string name)
	{
		_opts = Guard.Against.Null(opts);
		_appRepo = Guard.Against.Null(appRepo);
		_bus = Guard.Against.Null(bus);
		_windowResolver = Guard.Against.Null(windowResolver);
		_toggler = Guard.Against.Null(toggler);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_optionsAccessor = Guard.Against.Null(optionsAccessor);

		Name = Guard.Against.NullOrWhiteSpace(name);

		// TODO: Extract method.
		_bus.OnEvent<WtqWindowFocusEvent>(
			async e =>
			{
				// This app lost focus.
				if (e.LostFocusWindow == Window)
				{
					// If we lost focus to another WTQ-managed app, always close.
					var otherApp = e.GotFocusWindow != null ? _appRepo.GetByWindow(e.GotFocusWindow) : null;
					if (otherApp != null && otherApp != this)
					{
						await CloseAsync().NoCtx();
					}

					// Only close if we should on focus lost.
					if (!_opts.CurrentValue.GetHideOnFocusLostForApp(optionsAccessor()))
					{
						_log.LogDebug("App '{App}' lost focus, but 'hide on focus lost' is disabled", this);
						return;
					}

					// TODO: Setting for "hide on focus lost, only if newly focussed window is on the same screen".
					_log.LogDebug("App '{App}' lost focus, closing", this);

					await CloseAsync().NoCtx();
				}
			});

		_bus.OnEvent<WtqAppToggledEvent>(
			async e =>
			{
				// This app got toggled.
				if (e.App == this)
				{
					// TODO: Wait for other apps to close?

					await ToggleAsync().NoCtx();
					return;
				}

				// TODO: Is close necessary, eg. not already handled through focus lost?
			});

		// TODO: Extract method.
		_ = Task.Run(
			async () =>
			{
				while (true)
				{
					await UpdateLocalAppStateAsync(allowStartNew: false).NoCtx();

					await Task.Delay(TimeSpan.FromSeconds(1)).NoCtx();
				}
			});
	}

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// </summary>
	public bool IsAttached => Window?.IsValid ?? false;

	public bool IsOpen { get; private set; }

	public string Name { get; }

	public WtqAppOptions Options => _optionsAccessor();

	public WtqWindow? Window { get; private set; }

	public async Task ToggleAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		await (IsOpen ? CloseAsync(mods) : OpenAsync(mods)).NoCtx();
	}

	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		IsOpen = false;

		if (!IsAttached)
		{
			_log.LogWarning("Attempted to close inactive app {App}", this);
			return;
		}

		_log.LogInformation("Closing app '{App}'", this);

		// Move window off-screen.
		await _toggler.ToggleOffAsync(this, mods).ConfigureAwait(false);

		// Hide window.
		await Window.SetVisibleAsync(false).ConfigureAwait(false);

		await UpdateWindowPropsAsync().NoCtx();
	}

	public async ValueTask DisposeAsync()
	{
		// TODO: Add ability to close attached processes when app closes.
		if (!IsAttached)
		{
			return;
		}

		// Restore original position.
		// TODO: Restore to original position (when we got a hold of the process).
		// var bounds = new Rectangle(); //await Process.WindowRect;
		// bounds.Width = 1280;
		// bounds.Height = 800;
		// bounds.X = 10;
		// bounds.Y = 10;
		//
		// _log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

		// Toggle app onto the screen again.
		await OpenAsync(ToggleModifiers.Instant).NoCtx();

		// Reset app props.
		await ResetPropsAsync().NoCtx();
	}

	/// <summary>
	/// Returns the rectangle of the screen that the app is on.<br/>
	/// Uses the top-left corner of the app window to look for the corresponding screen,
	/// which is useful to keep in mind when using multiple screens.
	/// </summary>
	public async Task<Rectangle> GetScreenRectAsync()
	{
		_log.LogTrace("Looking for current screen rect for app {App}", this);

		// Get All screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get window rect of this app.
		var windowRect = await GetWindowRectAsync().NoCtx();

		// Look for screen rect that contains the left-top corner of the app window.
		foreach (var screenRect in screenRects)
		{
			if (screenRect.Contains(windowRect.Location))
			{
				_log.LogTrace("Got screen {Screen}, for app {App}", screenRect, this);
				return screenRect;
			}
			else
			{
				_log.LogTrace("Screen {Screen} does NOT contain app {App}", screenRect, this);
			}
		}

		_log.LogWarning("Could not find screen for app {App}, returning primary screen", this);

		return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
	}

	public async Task<Rectangle> GetWindowRectAsync()
	{
		if (Window == null || !IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		return await Window.GetWindowRectAsync().NoCtx();
	}

	public async Task MoveWindowAsync(Point location)
	{
		if (Window == null || !IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		await Window.MoveToAsync(location).NoCtx();
	}

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		_log.LogInformation("Opening app '{App}'", this);

		IsOpen = true;

		await UpdateLocalAppStateAsync(allowStartNew: true).NoCtx();
		await UpdateWindowPropsAsync().NoCtx();

		// If we are not attached to any window, stop the "Open" action, as we don't have anything to open.
		if (!IsAttached)
		{
			return false;
		}

		// Make sure the app window is visible and has focus.
		await Window!.SetVisibleAsync(true).NoCtx();
		await Window.BringToForegroundAsync().NoCtx();

		// Move app onto screen.
		await _toggler.ToggleOnAsync(this, mods).NoCtx();

		return true;
	}

	public async Task ResizeWindowAsync(Size size)
	{
		if (Window == null || !IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		await Window.ResizeAsync(size).NoCtx();
	}

	public override string ToString() => $"[App:{Options}] {Window?.ToString() ?? "<no process>"}";

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateLocalAppStateAsync(bool allowStartNew)
	{
		// Check that if we have a process handle, the process is still active.
		if (IsAttached)
		{
			_log.LogTrace("Window handle '{Window}' for app '{App}' is still active, skipping update", Window, this);
			return;
		}

		// If we don't have a process handle, see if we can get one.
		_log.LogInformation("No process attached to app {App}, asking process factory for one now", this);

		// Ask the process factory for a new handle.
		var process = await _windowResolver.GetWindowHandleAsync(Options, allowStartNew).NoCtx();

		// Log a warning if we don't have a process handle at this point.
		if (process == null)
		{
			_log.LogWarning("No process instances found for app '{App}'", Options);
			return;
		}

		// We didn't have a process handle when we got into this method, but we have one now,
		// so attach to the newly acquired handle.
		_log.LogInformation("Got process for app {App}, attaching", this);

		await AttachToWindowAsync(process).ConfigureAwait(false);
	}

	/// <summary>
	/// Stores the specified <paramref name="window"/>'s handle, and toggles it off the screen.
	/// </summary>
	private async Task AttachToWindowAsync(WtqWindow window)
	{
		Guard.Against.Null(window);

		_log.LogInformation("Attaching to window handle '{Window}' for app '{App}'", window, this);

		Window = window;

		// Move the window off the screen ASAP (e.g. without animating).
		await CloseAsync(ToggleModifiers.Instant).NoCtx();
	}

	/// <summary>
	/// Updates app properties such as taskbar icon visibility and opacity.
	/// </summary>
	private async Task UpdateWindowPropsAsync()
	{
		if (!IsAttached)
		{
			return;
		}

		// Always on top.
		await Window!.SetAlwaysOnTopAsync(_opts.CurrentValue.GetAlwaysOnTopForApp(Options)).NoCtx();

		// Opacity.
		await Window.SetTransparencyAsync(_opts.CurrentValue.GetOpacityForApp(Options)).NoCtx();

		// Window Title.
		var title = Options.WindowTitleOverride;
		if (!string.IsNullOrWhiteSpace(title))
		{
			await Window.SetWindowTitleAsync(title).NoCtx();
		}

		// Taskbar icon visibility.
		switch (_opts.CurrentValue.GetTaskbarIconVisibilityForApp(Options))
		{
			case TaskBarIconVisibility.AlwaysHidden:
				await Window.SetTaskbarIconVisibleAsync(false).NoCtx();
				break;
			case TaskBarIconVisibility.AlwaysVisible:
				await Window.SetTaskbarIconVisibleAsync(true).NoCtx();
				break;
			case TaskBarIconVisibility.WhenAppVisible:
				await Window.SetTaskbarIconVisibleAsync(IsOpen).NoCtx();
				break;
			default:
				await Window.SetTaskbarIconVisibleAsync(true).NoCtx();
				break;
		}
	}

	/// <summary>
	/// Resets app properties such as taskbar icon visibility and opacity.
	/// </summary>
	private async Task ResetPropsAsync()
	{
		if (!IsAttached)
		{
			return;
		}

		// Restore "always on top" state.
		await Window!.SetAlwaysOnTopAsync(false).NoCtx();

		// Restore taskbar icon visibility.
		await Window.SetTaskbarIconVisibleAsync(true).NoCtx();

		// Restore opacity.
		await Window.SetTransparencyAsync(100).NoCtx();
	}
}