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
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqProcessFactory _procFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqScreenInfoProvider _screenInfoProvider;
	private readonly IWtqAppToggleService _toggler;

	public WtqApp(
		IOptionsMonitor<WtqOptions> opts,
		IWtqProcessFactory procFactory,
		IWtqProcessService procService,
		IWtqScreenInfoProvider screenInfoProvider,
		IWtqAppToggleService toggler,
		Func<WtqAppOptions> optionsAccessor,
		string name)
	{
		_opts = Guard.Against.Null(opts);
		_procFactory = Guard.Against.Null(procFactory);
		_procService = Guard.Against.Null(procService);
		_toggler = Guard.Against.Null(toggler);
		_screenInfoProvider = Guard.Against.Null(screenInfoProvider);
		_optionsAccessor = Guard.Against.Null(optionsAccessor);

		Name = Guard.Against.NullOrWhiteSpace(name);
	}

	/// <summary>
	/// Whether an active process is being tracked by this app instance.
	/// TODO: Include check for whether the process has been killed etc.
	/// </summary>
	public bool IsActive => Process != null;

	public string Name { get; }

	public WtqAppOptions Options => _optionsAccessor();

	public WtqWindow? Process { get; private set; }

	public string? ProcessDescription => Process == null
		? "<no process attached>"
		: Process.ToString();

	public async Task AttachAsync(WtqWindow process)
	{
		Guard.Against.Null(process);

		Process = process;

		// TODO: Configurable.
		if (_opts.CurrentValue.GetTaskbarIconVisibilityForApp(Options) == TaskBarIconVisibility.AlwaysHidden)
		{
			await process.SetTaskbarIconVisibleAsync(false).NoCtx();
		}

		await CloseAsync(ToggleModifiers.Instant).ConfigureAwait(false);

		_log.LogInformation("Found process instance for app '{App}': '{Process}'", Options, process);
	}

	public async Task CloseAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		if (!IsActive)
		{
			_log.LogWarning("Attempted to close inactive app {App}", this);
			return;
		}

		_log.LogInformation("Closing app '{App}'", this);

		await _toggler.ToggleOffAsync(this, mods).ConfigureAwait(false);

		await Process.SetVisibleAsync(false).ConfigureAwait(false);
	}

	public async ValueTask DisposeAsync()
	{
		// TODO: Add ability to close attached processes when app closes.
		if (Process != null)
		{
			// Restore original position.
			// TODO: Restore to original position (when we got a hold of the process).
			var bounds = Process.WindowRect;
			bounds.Width = 1280;
			bounds.Height = 800;
			bounds.X = 10;
			bounds.Y = 10;

			_log.LogInformation("Restoring process '{Process}' to its original bounds of '{Bounds}'", ProcessDescription, bounds);

			// Toggle app onto the screen again.
			await OpenAsync(ToggleModifiers.Instant).NoCtx();

			// Restore "always on top" state.
			await Process.SetAlwaysOnTopAsync(false).NoCtx();

			// Restore taskbar icon visibility.
			await Process.SetTaskbarIconVisibleAsync(true).NoCtx();
		}
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
		var windowRect = GetWindowRect();

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

	[SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "MvdO: May throw an exception, which we don't want to do in a property.")]
	public Rectangle GetWindowRect()
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		return Process.WindowRect;
	}

	public async Task MoveWindowAsync(Rectangle rect)
	{
		if (Process == null)
		{
			throw new InvalidOperationException($"App '{this}' does not have a process attached.");
		}

		await Process.MoveToAsync(rect: rect).NoCtx();
	}

	public async Task<bool> OpenAsync(ToggleModifiers mods = ToggleModifiers.None)
	{
		await UpdateAsync().ConfigureAwait(false);

		// If we have an active process attached, toggle it open.
		if (IsActive)
		{
			_log.LogInformation("Opening app '{App}'", this);

			// Make sure the app is visible and has focus.
			await Process.SetVisibleAsync(true).NoCtx();
			await Process.BringToForegroundAsync().NoCtx();

			await _toggler.ToggleOnAsync(this, mods).NoCtx();

			return true;
		}

		if (!IsActive && Options.AttachMode == AttachMode.Manual)
		{
			var pr = _procService.GetForegroundWindow();
			if (pr != null)
			{
				await AttachAsync(pr).ConfigureAwait(false);

				return true;
			}

			_log.LogWarning("ATTACH?!");
		}

		return false;
	}

	public override string ToString()
	{
		try
		{
			return $"[App:{Options}] {Process?.ToString() ?? "<no process>"}";
		}
		catch
		{
			return $"[App:{Options}] <no process>";
		}
	}

	/// <summary>
	/// Updates the state of the <see cref="WtqApp"/> object to reflect running processes on the system.
	/// </summary>
	public async Task UpdateAsync()
	{
		// Check that if we have a process handle, the process is still active.
		if (Process is { IsValid: false })
		{
			_log.LogInformation("Process '{Process}' exited, releasing handle", Process);
			Process = null;
		}

		// If we don't have a process handle, see if we can get one.
		if (Process == null)
		{
			_log.LogInformation("No process attached to app {App}, asking process factory for one now", this);

			// As the process factory for a new handle.
			var process = await _procFactory.GetProcessAsync(Options).NoCtx();

			// Log a warning if we don't have a process handle at this point.
			if (process == null)
			{
				_log.LogWarning("No process instances found for app '{App}'", Options);
				return;
			}

			// We didn't have a process handle when we got into this method, but we have one now,
			// so attach to the newly acquired handle.
			_log.LogInformation("Got process for app {App}, attaching", this);

			await AttachAsync(process).ConfigureAwait(false);
		}

		// TODO: Move to "AttachAsync"?
		if (Process != null && IsActive)
		{
			// Always on top.
			await Process.SetAlwaysOnTopAsync(_opts.CurrentValue.GetAlwaysOnTopForApp(Options)).NoCtx();

			// Opacity.
			await Process.SetTransparencyAsync(_opts.CurrentValue.GetOpacityForApp(Options)).NoCtx();
		}
	}
}