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
	private readonly WtqSemaphoreSlim _updateLock = new(1, 1);

	private readonly ILogger _log;

	private readonly Func<WtqAppOptions> _optionsAccessor;
	private readonly IWtqAppToggleService _toggler;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqWindowResolver _windowResolver;

	private Point? _lastLoc;

	public WtqApp(
		IWtqAppToggleService toggler,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqWindowResolver windowResolver,
		Func<WtqAppOptions> optionsAccessor,
		string name)
	{
		_log = Log.For($"{GetType()}|{name}");

		_windowResolver = Guard.Against.Null(windowResolver);
		_toggler = Guard.Against.Null(toggler);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_optionsAccessor = Guard.Against.Null(optionsAccessor);

		Name = Guard.Against.NullOrWhiteSpace(name);
	}

	public bool IsActive { get; set; } = true;

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
		IsActive = true;

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
		await SuspendAsync().NoCtx();
	}

	/// <summary>
	/// Returns the rectangle of the screen that the app is on.<br/>
	/// Uses the top-left corner of the app window to look for the corresponding screen,
	/// which is useful to keep in mind when using multiple screens.
	/// </summary>
	public async Task<Rectangle> GetScreenRectAsync()
	{
		if (!IsAttached)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		_log.LogDebug("Looking for current screen rect for app {App}", this);

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
				_log.LogDebug("Got screen {Screen}, for app {App}", screenRect, this);
				return screenRect;
			}

			_log.LogDebug("Screen {Screen} does NOT contain app {App}", screenRect, this);
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

		await Window.SetLocationAsync(location).NoCtx();

		_lastLoc = location;
	}

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		if (!IsOptionsValid())
		{
			return false;
		}

		_log.LogInformation("Opening app '{App}'", this);

		IsActive = true;

		if (IsOpen)
		{
			_log.LogWarning("App '{App}' is already open, skipping further actions", this);
			return false;
		}

		IsOpen = true;

		await UpdateLocalAppStateAsync(allowStartNew: true).NoCtx();
		await UpdateWindowPropsAsync().NoCtx();

		// If we are not attached to any window, stop the "Open" action, as we don't have anything to open.
		if (!IsAttached)
		{
			_log.LogWarning("App '{App}' is not attached, skipping further action", this);
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

		await Window.SetSizeAsync(size).NoCtx();
	}

	public async Task SuspendAsync()
	{
		IsActive = false;

		// TODO: Add ability to close attached processes when app closes.
		if (!IsAttached)
		{
			_log.LogInformation("App is not attached, not doing anything for cleanup");
			return;
		}

		// Stop update loop.
		await (_loop?.DisposeAsync() ?? ValueTask.CompletedTask).NoCtx();

		// Reset window location & size.
		await ResetLocationAndSizeAsync().NoCtx();

		// Reset app props.
		await ResetPropsAsync().NoCtx();
	}

	public override string ToString() => $"[App:{Options}] {Window?.ToString() ?? "<no process>"}";

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateLocalAppStateAsync(bool allowStartNew)
	{
		if (!IsOptionsValid())
		{
			return;
		}

		// Make sure this method always runs non-concurrently.
		using var l = await _updateLock.WaitAsync(Cancel.After(TimeSpan.FromSeconds(15))).ConfigureAwait(false);

		// Ask window to update its state first.
		await (Window?.UpdateAsync() ?? Task.CompletedTask).NoCtx();

		// Check that if we have a process handle, the process is still active.
		if (IsAttached)
		{
			await UpdateWindowPropsAsync().NoCtx();

			if (_lastLoc != null)
			{
				await CheckAndRestoreWindowRectAsync(Window, _lastLoc.Value).NoCtx();
			}

			_log.LogDebug("Window handle '{Window}' for app '{App}' is still active, skipping update", Window, this);
			return;
		}

		// If we don't have a window handle, see if we can get one.
		_log.LogDebug("No window attached to app {App}, asking window resolver for one now", this);

		// Ask the window resolver for a new handle.
		var window = await _windowResolver.GetWindowHandleAsync(Options, allowStartNew).NoCtx();

		// Log a warning if we don't have a window handle at this point.
		if (window == null)
		{
			_log.LogDebug("No window found for app '{App}'", Options);
			return;
		}

		// We didn't have a window handle when we got into this method, but we have one now,
		// so attach to the newly acquired handle.
		_log.LogInformation("Got window for app {App}, attaching", this);

		await AttachToWindowAsync(window).NoCtx();
	}

	/// <summary>
	/// App windows can sometimes move around the screen due to external interaction.<br/>
	/// For example, when the screen resolution changes, a monitor is (dis)connected, or when the shell restarts.<br/>
	/// <br/>
	/// Compares the current location of the app window, to the one we last set it to.<br/>
	/// If they don't match, moves it back to where we last put it.
	/// </summary>
	private async Task CheckAndRestoreWindowRectAsync(WtqWindow window, Point lastLoc)
	{
		if (!IsAttached)
		{
			return;
		}

		// Fetch current window location.
		var rect = await window.GetWindowRectAsync().NoCtx();

		// Check the distance between where we last left the window, and where it is now.
		var dist = rect.Location.DistanceTo(lastLoc);

		// Allow a little bit of drift, but restore location when the distance gets too large.
		if (dist > 10)
		{
			_log.LogWarning(
				"Window seems to have moved externally, restoring position (moved from {LastLoc} to {CurrentLoc}, distance of {Distance})",
				lastLoc,
				rect,
				dist);

			// Have the toggle re-do the toggling, with the current (possibly changed) screen setup.
			if (IsOpen)
			{
				await _toggler.ToggleOnAsync(this, ToggleModifiers.Instant).NoCtx();
			}
			else
			{
				await _toggler.ToggleOffAsync(this, ToggleModifiers.Instant).NoCtx();
			}
		}
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
		await Window.SetAlwaysOnTopAsync(Options.GetAlwaysOnTop()).NoCtx();

		// Opacity.
		await Window.SetTransparencyAsync(Options.GetOpacity()).NoCtx();

		// Window Title.
		var title = Options.WindowTitleOverride;
		if (!string.IsNullOrWhiteSpace(title))
		{
			await Window.SetWindowTitleAsync(title).NoCtx();
		}

		// Taskbar icon visibility.
		switch (Options.GetTaskbarIconVisibility())
		{
			case TaskbarIconVisibility.AlwaysHidden:
				await Window.SetTaskbarIconVisibleAsync(false).NoCtx();
				break;
			case TaskbarIconVisibility.AlwaysVisible:
				await Window.SetTaskbarIconVisibleAsync(true).NoCtx();
				break;
			case TaskbarIconVisibility.WhenAppVisible:
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

	/// <summary>
	/// Moves window back to the center of a screen, and resets to a (hopefully) convenient size.
	/// </summary>
	private async Task ResetLocationAndSizeAsync()
	{
		// Get the screen with the cursor, to move the window to.
		var scr = await _screenInfoProvider.GetScreenWithCursorAsync().NoCtx();

		// Calculate a convenient target location & size for the window.
		var dstSize = scr.Size.MultiplyF(.9f);
		var dstLoc = dstSize.CenterInRectangle(scr);

		_log.LogInformation("Resetting window location to {Location} and size to {Size}", dstLoc, dstSize);

		// Move first.
		await MoveWindowAsync(dstLoc).NoCtx();

		// ...then resize, since in some cases (at least on KWin), we can only resize visible windows it seems.
		await ResizeWindowAsync(dstSize).NoCtx();
	}

	private bool IsOptionsValid()
	{
		if (Options.IsValid)
		{
			return true;
		}

		_log.LogWarning("Options for app {App} not valid. Please check settings, and/or the GUI for suggestions", this);
		return false;
	}
}