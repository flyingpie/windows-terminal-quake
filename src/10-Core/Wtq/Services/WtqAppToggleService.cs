namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppToggleService"/>
public class WtqAppToggleService(
	IWtqTargetScreenRectProvider targetScreenRectProvider,
	IWtqWindowRectProvider windowRectProvider,
	IWtqTween tween)
	: IWtqAppToggleService
{
	/// <summary>
	/// When moving app windows onto- and off the screen, we may not find a free space (i.e., the would-be locations are occupied by other screens).<br/>
	/// In these cases, we fall back to a location off in the distance, and instantly move the app window there.<br/>
	/// <br/>
	/// Values under 32K can cause issues in Windows, as they seem to be capped to -32k (causes location restore routine to start bouncing).
	/// </summary>
	private static readonly Point BehindScreenLocation = new(0, -30_000);

	private readonly ILogger _log = Log.For<WtqAppToggleService>();
	private readonly IWtqTargetScreenRectProvider _targetScreenRectProvider = Guard.Against.Null(targetScreenRectProvider);
	private readonly IWtqWindowRectProvider _windowRectProvider = Guard.Against.Null(windowRectProvider);
	private readonly IWtqTween _tween = Guard.Against.Null(tween);

	/// <inheritdoc/>
	public async Task ToggleOnAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Animation duration.
		var durationMs = GetDurationMs(app, mods);

		// Get rect of the screen where the app needs to go to.
		// Usually the screen with the cursor.
		var screenRectDst = await _targetScreenRectProvider.GetTargetScreenRectAsync(app.Options).NoCtx();

		// Get current window rect.
		// Used to grab the current size, if we're not allowed to resize the window.
		var windowRectCur = await app.GetWindowRectAsync().NoCtx();

		// Source & target rects.
		// Note that "current" and "source" rects are often the same, but _can_ be different.
		//
		// For example, when a window has toggled off of a different screen than where we're toggling it onto.
		// In such cases, the "current" rect could be above screen 1, where the "source" rect could be above screen 2.
		//
		// If we'd just move from wherever the window was left when toggled off last, the window could move from above
		// the first screen, to across the second, diagonally.
		//
		// Another case where these could be different, is when the available screens have changed since
		// last toggle (i.e. when a screen has been (dis)connected).
		//
		// By explicitly determining a new, and possibly different "source" rect, we get more consistent and
		// predictable animations.
		//
		// The "dest" rect, where we want the app to move to.
		var windowRectDst = await _windowRectProvider.GetOnScreenRectAsync(screenRectDst, windowRectCur, app.Options).NoCtx();

		// The "source" rect, where the window is coming from.
		var windowRectSrc = await _windowRectProvider.GetOffScreenRectAsync(screenRectDst, windowRectDst, app.Options).NoCtx();

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
				animType: app.Options.GetAnimationTypeToggleOn(),
				move: app.MoveWindowAsync)
			.NoCtx();

		// Resize window.
		await app.ResizeWindowAsync(windowRectDst.Size).NoCtx();
	}

	/// <inheritdoc/>
	public async Task ToggleOffAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Animation duration.
		var durationMs = GetDurationMs(app, mods);

		// Get the screen rectangle.
		// Used to find free space we can move the window to.
		var screenRectSrc = await app.GetScreenRectAsync().NoCtx();

		// Source & target rects.
		var windowRectSrc = await app.GetWindowRectAsync().NoCtx();
		var windowRectDst = await _windowRectProvider.GetOffScreenRectAsync(screenRectSrc: screenRectSrc, windowRectSrc: windowRectSrc, app.Options).NoCtx();

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
				animType: app.Options.GetAnimationTypeToggleOff(),
				move: app.MoveWindowAsync)
			.NoCtx();
	}

	/// <summary>
	/// Returns the time the animation should take in milliseconds.
	/// </summary>
	private static int GetDurationMs(WtqApp app, ToggleModifiers mods)
	{
		switch (mods)
		{
			case ToggleModifiers.Instant:
				return 0;

			case ToggleModifiers.SwitchingApps:
				return app.Options.GetAnimationDurationMsWhenSwitchingApps();

			case ToggleModifiers.None:
			default:
				return app.Options.GetAnimationDurationMs();
		}
	}
}