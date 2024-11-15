using Wtq.Services;

namespace Wtq;

/// <summary>
/// An "app" represents a single window that can be toggled (such as Windows Terminal).<br/>
///
/// Some traits of an "app":
/// - It is defined through the WTQ settings;
/// - It tracks its own state;
/// - It does not necessarily have a window attached;
/// - It can start a process, if configured as such;
/// - It has methods to do actions like open/close the window, set opacity, etc.
/// </summary>
public sealed class WtqApp : IAsyncDisposable
{
	private readonly ILogger _log = Log.For<WtqApp>();

	private readonly Func<WtqAppOptions> _optionsAccessor;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppToggleService _toggler;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqWindowResolver _windowResolver;

	private readonly Worker _loop;

	private Rectangle? _originalRect;

	public WtqApp(
		IOptionsMonitor<WtqOptions> opts,
		IWtqAppToggleService toggler,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqWindowResolver windowResolver,
		Func<WtqAppOptions> optionsAccessor,
		string name)
	{
		_opts = Guard.Against.Null(opts);
		_windowResolver = Guard.Against.Null(windowResolver);
		_toggler = Guard.Against.Null(toggler);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_optionsAccessor = Guard.Against.Null(optionsAccessor);

		Name = Guard.Against.NullOrWhiteSpace(name);

		// Start loop that updates app state periodically.
		_loop = new(
			$"{nameof(WtqApp)}.{name}",
			_ => UpdateLocalAppStateAsync(allowStartNew: false),
			TimeSpan.FromSeconds(1));
	}

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Window))]
	public bool IsAttached => Window?.IsValid ?? false;

	/// <summary>
	/// Whether the app is currently toggled onto the screen.<br/>
	/// Starts in the "true" state, as we presume the window is on-screen when we attach to it.
	/// </summary>
	public bool IsOpen { get; private set; } = true;

	/// <summary>
	/// The name of the app, as configured in the settings file.<br/>
	/// Used for logging purposes, and correlating across configuration changes.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Returns the <see cref="WtqAppOptions"/> associated with this app.
	/// </summary>
	public WtqAppOptions Options => _optionsAccessor();

	/// <summary>
	/// The <see cref="WtqWindow"/> that is tracked by this app (if any).
	/// </summary>
	public WtqWindow? Window { get; private set; }

	/// <summary>
	/// Toggle the app off the screen.
	/// </summary>
	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		if (!IsOpen)
		{
			return;
		}

		IsOpen = false;

		if (!IsAttached)
		{
			_log.LogWarning("Attempted to close inactive app {App}", this);
			return;
		}

		_log.LogInformation("Closing app '{App}'", this);

		// Move window off-screen.
		await _toggler.ToggleOffAsync(this, mods).NoCtx();

		await UpdateWindowPropsAsync().NoCtx();
	}

	public async ValueTask DisposeAsync()
	{
		// TODO: Add ability to close attached processes when app closes.
		if (!IsAttached)
		{
			return;
		}

		// Toggle app onto the screen again.
		await OpenAsync(ToggleModifiers.Instant).NoCtx();

		// Restore original position.
		if (_originalRect.HasValue)
		{
			await ResizeWindowAsync(_originalRect.Value.Size).NoCtx();
			await MoveWindowAsync(_originalRect.Value.Location).NoCtx();
		}

		// Reset app props.
		await ResetPropsAsync().NoCtx();

		await _loop.DisposeAsync().NoCtx();
	}

	/// <summary>
	/// Returns the rectangle of the screen that the app is on.<br/>
	/// Uses the top-left corner of the app window to look for the corresponding screen,
	/// which is useful to keep in mind when using multiple screens.
	/// </summary>
	public async Task<Rectangle> GetScreenRectAsync()
	{
		_log.LogTrace("Looking for current screen rect for app {App}", this);

		// Get all screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get window rect of this app.
		var windowRect = await GetWindowRectAsync().NoCtx();

		// Look for screen rect that contains the left-top corner of the app window.
		// TODO: Use screen with largest overlap instead?
		foreach (var screenRect in screenRects)
		{
			if (screenRect.Contains(windowRect.Location))
			{
				_log.LogTrace("Got screen {Screen}, for app {App}", screenRect, this);
				return screenRect;
			}

			_log.LogTrace("Screen {Screen} does NOT contain app {App}", screenRect, this);
		}

		_log.LogWarning("Could not find screen for app {App} ({Rectangle}), returning primary screen", this, windowRect);

		return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
	}

	public async Task<Rectangle> GetWindowRectAsync()
	{
		if (!IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		return await Window.GetWindowRectAsync().NoCtx();
	}

	public async Task MoveWindowAsync(Point location)
	{
		if (!IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		await Window.MoveToAsync(location).NoCtx();
	}

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		if (IsOpen)
		{
			return false;
		}

		_log.LogInformation("Opening app '{App}'", this);

		IsOpen = true;

		await UpdateLocalAppStateAsync(allowStartNew: true).NoCtx();
		await UpdateWindowPropsAsync().NoCtx();

		// If we are not attached to any window, stop the "Open" action, as we don't have anything to open.
		if (!IsAttached)
		{
			return false;
		}

		// Make sure the app has focus.
		await Window.BringToForegroundAsync().NoCtx();

		// Move app onto screen.
		await _toggler.ToggleOnAsync(this, mods).NoCtx();

		return true;
	}

	public async Task ResizeWindowAsync(Size size)
	{
		if (!IsAttached)
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

		// If we don't have a window handle, see if we can get one.
		_log.LogInformation("No window attached to app {App}, asking window resolver for one now", this);

		// Ask the window resolver for a new handle.
		var window = await _windowResolver.GetWindowHandleAsync(Options, allowStartNew).NoCtx();

		// Log a warning if we don't have a window handle at this point.
		if (window == null)
		{
			_log.LogWarning("No window found for app '{App}'", Options);
			return;
		}

		// We didn't have a window handle when we got into this method, but we have one now,
		// so attach to the newly acquired handle.
		_log.LogInformation("Got window for app {App}, attaching", this);

		await AttachToWindowAsync(window).NoCtx();
	}

	/// <summary>
	/// Stores the specified <paramref name="window"/>'s handle, and toggles it off the screen.
	/// </summary>
	private async Task AttachToWindowAsync(WtqWindow window)
	{
		Guard.Against.Null(window);

		_log.LogInformation("Attaching to window handle '{Window}' for app '{App}'", window, this);

		Window = window;

		_originalRect = await window.GetWindowRectAsync().NoCtx();

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
		await Window.SetAlwaysOnTopAsync(_opts.CurrentValue.GetAlwaysOnTopForApp(Options)).NoCtx();

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
		await Window.SetAlwaysOnTopAsync(false).NoCtx();

		// Restore taskbar icon visibility.
		await Window.SetTaskbarIconVisibleAsync(true).NoCtx();

		// Restore opacity.
		await Window.SetTransparencyAsync(100).NoCtx();
	}
}