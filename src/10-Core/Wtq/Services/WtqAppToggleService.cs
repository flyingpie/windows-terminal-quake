using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppToggleService"/>
public class WtqAppToggleService(
	IOptionsMonitor<WtqOptions> opts,
	IWtqScreenInfoProvider screenInfoProvider,
	IWtqTween tween)
	: IWtqAppToggleService
{
	/// <summary>
	/// When moving app windows onto- and off the screen, we may not find a free space (i.e., the would-be locations are occupied by other screens).<br/>
	/// In these cases, we fall back to a location off in the distance, and instantly move the app window there.
	/// </summary>
	private static readonly Point BehindScreenLocation = new(0, -100_000);

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

		// All available screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get rect of the screen where the app currently is.
		var screenRect = await GetTargetScreenRectAsync(app).NoCtx();

		// Source & target rects.
		var windowRectSrc = GetOffScreenWindowRect(app, screenRect, screenRects);
		var windowRectDst = GetOnScreenWindowRect(app, screenRect);

		_log.LogDebug("ToggleOn app '{App}' from '{From}' to '{To}'", app, windowRectSrc, windowRectDst);

		// If no off-screen location could be found, just blink it into existence instantly.
		if (windowRectSrc == null)
		{
			_log.LogWarning("Could not find an off-screen location to tween from, fallback to instant-on");

			// Note that on KWin, it seems we're only allowed to resize windows when they're on-screen.
			// So move first, then resize.
			await app.MoveWindowAsync(windowRectDst.Location).NoCtx();
			await app.ResizeWindowAsync(windowRectDst.Size).NoCtx();
			return;
		}

		// Resize window.
		await app.ResizeWindowAsync(windowRectDst.Size).NoCtx();

		// Move window.
		await _tween
			.AnimateAsync(
				src: windowRectSrc.Value.Location,
				dst: windowRectDst.Location,
				durationMs: durationMs,
				animType: _opts.CurrentValue.GetAnimationTypeToggleOn(app.Options),
				move: app.MoveWindowAsync)
			.NoCtx();
	}

	/// <inheritdoc/>
	public async Task ToggleOffAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Animation duration.
		var durationMs = GetDurationMs(mods);

		// All available screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get rect of the screen where the app currently is.
		var screenRect = await app.GetScreenRectAsync().NoCtx();

		// Source & target rects.
		var windowRectSrc = await app.GetWindowRectAsync().NoCtx();
		var windowRectDst = GetOffScreenWindowRect(app, screenRect, screenRects);

		_log.LogDebug("ToggleOff app '{App}' from '{From}' to '{To}'", app, windowRectSrc, windowRectDst);

		// If no off-screen location could be found, just blink it out of existence instantly.
		if (windowRectDst == null)
		{
			_log.LogWarning("Could not find an off-screen location to tween to, fallback to instant-off");

			// Note that on KWin, it seems we're only allowed to resize windows when they're on-screen.
			// So move first, then resize.
			await app.MoveWindowAsync(BehindScreenLocation).NoCtx();
			return;
		}

		// Resize window.
		await app.ResizeWindowAsync(windowRectDst.Value.Size).NoCtx();

		// Move window.
		await _tween
			.AnimateAsync(
				src: windowRectSrc.Location,
				dst: windowRectDst.Value.Location,
				durationMs: durationMs,
				animType: _opts.CurrentValue.GetAnimationTypeToggleOff(app.Options),
				move: app.MoveWindowAsync)
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
				return 250;
				// return _opts.CurrentValue.AnimationDurationMs;
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
	private Rectangle? GetOffScreenWindowRect(
		WtqApp app,
		Rectangle currScreenRect,
		Rectangle[] screenRects)
	{
		Guard.Against.Null(app);
		Guard.Against.Null(screenRects);

		// Get the app's current window rectangle.
		var windowRect = GetOnScreenWindowRect(app, currScreenRect);

		// Get possible rectangles to move the app to.
		var targetRects = GetOffScreenWindowRects(app, windowRect, currScreenRect);

		// Return first target rectangle that does not overlap with a screen.
		var targetRect = targetRects
			.FirstOrDefault(r => !screenRects.Any(scr => scr.IntersectsWith(r)));

		// Return either the rectangle that we found, or null in case no free screen space is available.
		return !targetRect.IsEmpty ? targetRect : null;
	}

	/// <summary>
	/// Returns a set of <see cref="Rectangle"/>s, each a possible off-screen position for the <paramref name="windowRect"/> to move to.<br/>
	/// The list is ordered by <see cref="OffScreenLocation"/>, as specified in the settings.
	/// </summary>
	private Rectangle[] GetOffScreenWindowRects(
		WtqApp app,
		Rectangle windowRect,
		Rectangle screenRect)
	{
		Guard.Against.Null(app);
		Guard.Against.Null(windowRect);
		Guard.Against.Null(screenRect);

		var margin = 100;

		return _opts.CurrentValue
			.GetOffScreenLocationsForApp(app.Options)
			.Select(dir => dir switch
			{
				Above or None => windowRect with
				{
					// Top of the screen, minus height of the app window.
					Y = screenRect.Y - windowRect.Height - margin,
				},

				Below => windowRect with
				{
					// Bottom of the screen.
					Y = screenRect.Y + screenRect.Height + margin,
				},

				Left => windowRect with
				{
					// Left of the screen, minus width of the app window.
					X = screenRect.X - windowRect.Width - margin,
				},

				Right => windowRect with
				{
					// Right of the screen, plus width of the app window.
					X = screenRect.X + screenRect.Width + windowRect.Width + margin,
				},

				_ => throw new WtqException("Unknown toggle direction."),
			})
			.ToArray();
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
				var screenIndex = _opts.CurrentValue.GetMonitorIndex(app.Options);

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