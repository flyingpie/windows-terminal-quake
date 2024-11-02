namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppToggleService"/>
public class WtqAppToggleService(
	IOptionsMonitor<WtqOptions> opts,
	IWtqScreenInfoProvider screenInfoProvider,
	IWtqTween tween)
	: IWtqAppToggleService
{
	private readonly ILogger _log = Log.For<WtqAppToggleService>();
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqScreenInfoProvider _screenInfoProvider = Guard.Against.Null(screenInfoProvider);
	private readonly IWtqTween _tween = Guard.Against.Null(tween);

	/// <inheritdoc/>
	public async Task ToggleOnAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Animation duration.
		var durationMs = GetDurationMs(mods);

		// Screen bounds.
		var screenRect = await GetTargetScreenRectAsync(app).NoCtx();

		// Source & target bounds.
		var windowRectSrc = await GetOffScreenWindowRectAsync(app, screenRect).NoCtx();
		var windowRectDst = GetOnScreenWindowRect(app, screenRect);

		// Move window.
		_log.LogDebug("ToggleOn app '{App}' from '{From}' to '{To}'", app, windowRectSrc, windowRectDst);

		await app.ResizeWindowAsync(windowRectDst.Size).NoCtx();

		await _tween
			.AnimateAsync(
				src: windowRectSrc.Location,
				dst: windowRectDst.Location,
				durationMs: durationMs,
				animType: _opts.CurrentValue.AnimationTypeToggleOn,
				move: app.MoveWindowAsync)
			.NoCtx();
	}

	/// <inheritdoc/>
	public async Task ToggleOffAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Animation duration.
		var durationMs = GetDurationMs(mods);

		// Screen bounds.
		var screenRect = await app.GetScreenRectAsync().NoCtx();

		// Source & target bounds.
		var windowRectSrc = await app.GetWindowRectAsync().NoCtx();
		var windowRectDst = await GetOffScreenWindowRectAsync(app, screenRect).NoCtx();

		_log.LogDebug("ToggleOff app '{App}' from '{From}' to '{To}'", app, windowRectSrc, windowRectDst);

		await app.ResizeWindowAsync(windowRectDst.Size).NoCtx();

		await _tween
			.AnimateAsync(
				windowRectSrc.Location,
				windowRectDst.Location,
				durationMs,
				_opts.CurrentValue.AnimationTypeToggleOff,
				app.MoveWindowAsync)
			.NoCtx();
	}

	/// <summary>
	/// Returns the time the animation should take in milliseconds.
	/// </summary>
	private int GetDurationMs(ToggleModifiers mods)
	{
		switch (mods)
		{
			case ToggleModifiers.Instant:
				return 0;

			case ToggleModifiers.SwitchingApps:
				return _opts.CurrentValue.AnimationDurationMsWhenSwitchingApps;

			case ToggleModifiers.None:
			default:
				return _opts.CurrentValue.AnimationDurationMs;
		}
	}

	/// <summary>
	/// Get the position rect a window should be when on-screen.
	/// </summary>
	private Rectangle GetOnScreenWindowRect(WtqApp app, Rectangle screenRect)
	{
		Guard.Against.Null(app);

		// Calculate app window size.
		var windowWidth = (int)(screenRect.Width * _opts.CurrentValue.HorizontalScreenCoverageIndexForApp(app.Options));
		var windowHeight = (int)(screenRect.Height * _opts.CurrentValue.VerticalScreenCoverageIndexForApp(app.Options));

		// Calculate horizontal position.
		var x = app.Options.HorizontalAlign switch
		{
			// Left
			HorizontalAlign.Left => screenRect.X,

			// Right
			HorizontalAlign.Right => screenRect.X + (screenRect.Width - windowWidth),

			// Center
			_ => screenRect.X + (int)Math.Ceiling((screenRect.Width / 2f) - (windowWidth / 2f)),
		};

		return new Rectangle()
		{
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			X = x,

			// Y, top of the screen + offset
			Y = screenRect.Y + (int)_opts.CurrentValue.GetVerticalOffsetForApp(app.Options),

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			Width = windowWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			Height = windowHeight,
		};
	}

	/// <summary>
	/// Get the position rect a window should be when off-screen.
	/// </summary>
	private async Task<Rectangle> GetOffScreenWindowRectAsync(WtqApp app, Rectangle screenRect)
	{
		Guard.Against.Null(app);

		var windowRect = GetOnScreenWindowRect(app, screenRect);

		var screens = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();
		var dirs = _opts.CurrentValue.GetToggleDirectionOrderForApp(app.Options);

		foreach (var dir in dirs)
		{
			// TODO: Detect if the calculated target rect is empty (e.g. not within a screen).
		}

		// TODO: What if directions is empty? Handle that in the options thing instead of here?
		var d = dirs.First();

		switch (d)
		{
			case ToggleDirection.Down:
				windowRect.Y
					= screenRect.Y + screenRect.Height // Bottom of the screen (which can be negative, when on the non-primary screen).
					+ windowRect.Height // Plus height of the app window.
					+ 100; // Plus a little margin.
				break;
			case ToggleDirection.Up:
				windowRect.Y
					= screenRect.Y // Top of the screen (which can be negative, when on the non-primary screen).
					- windowRect.Height // Minus height of the app window.
					- 100; // Minus a little margin.
				break;
			case ToggleDirection.Left:
				// TODO
				break;
			case ToggleDirection.Right:
				// TODO
				break;
		}

		return windowRect;
	}

	private IEnumerable<Rectangle> GetOffScreenWindowRects()
	{
		// TODO: Take order into account.
		return null;
	}

	/// <summary>
	/// Returns the rectangle of the screen that should be used to toggle the <paramref name="app"/> to.
	/// </summary>
	private async Task<Rectangle> GetTargetScreenRectAsync(WtqApp app)
	{
		Guard.Against.Null(app);

		_log.LogTrace("Looking for target screen rect for app {App}", this);

		// Determine what monitor we want to use.
		var preferMonitor = app.Options.PreferMonitor ?? _opts.CurrentValue.PreferMonitor;

		switch (preferMonitor)
		{
			case PreferMonitor.AtIndex:
			{
				// Get configured screen index.
				var screenIndex = app.Options.MonitorIndex ?? _opts.CurrentValue.MonitorIndex;

				_log.LogTrace("Using screen with index {Index}", screenIndex);

				var screens = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

				if (screens.Length > screenIndex)
				{
					var screen = screens[screenIndex];

					_log.LogTrace("Found screen {Screen} with index {Index}", screen, screenIndex);

					return screen;
				}

				_log.LogWarning(
					"Option '{OptionName}' was set to {ScreenIndex}, but only {ScreenCount} screens were found (note that the value starts at 0, not 1), falling back to primary",
					nameof(app.Options.MonitorIndex),
					screenIndex,
					screens.Length);

				return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
			}

			case PreferMonitor.Primary:
			{
				_log.LogTrace("Using primary screen");

				return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
			}

			case PreferMonitor.WithCursor:
			{
				_log.LogTrace("Using screen with cursor");

				return await _screenInfoProvider.GetScreenWithCursorAsync().NoCtx();
			}

			default:
			{
				_log.LogWarning(
					"Unknown value '{OptionValue}' for option '{OptionName}', falling back to primary",
					preferMonitor,
					nameof(app.Options.PreferMonitor));

				return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
			}
		}
	}
}