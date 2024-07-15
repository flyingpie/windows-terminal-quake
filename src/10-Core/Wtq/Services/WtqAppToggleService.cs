using Wtq.Data;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqAppToggleService"/>
public class WtqAppToggleService(
	IOptionsMonitor<WtqOptions> opts,
	IWtqTween tween,
	IWtqScreenInfoProvider scrInfoProvider)
	: IWtqAppToggleService
{
	private readonly ILogger _log = Log.For<WtqAppToggleService>();
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqScreenInfoProvider _scrInfoProvider = Guard.Against.Null(scrInfoProvider);
	private readonly IWtqTween _tween = Guard.Against.Null(tween);

	/// <inheritdoc/>
	public async Task ToggleOffAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		var durationMs = GetDurationMs(app, mods);
		var from = app.GetWindowRect();
		var to = GetToggleOffToWindowRect(from);

		_log.LogInformation("ToggleOff from '{From}' to '{To}'", from, to);

		await _tween.AnimateAsync(from, to, durationMs, AnimationType.EaseInQuart, app.MoveWindow).ConfigureAwait(false);
	}

	/// <inheritdoc/>
	public async Task ToggleOnAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		// Make sure the app has focus.
		app.BringToForeground();

		var durationMs = GetDurationMs(app, mods);
		var screen = GetToggleOnToScreenRect(app);
		var to = GetToggleOnToWindowRect(app, screen);
		var from = GetToggleOnFromWindowRect(to);

		await _tween.AnimateAsync(from, to, durationMs, AnimationType.EaseOutQuart, app.MoveWindow).ConfigureAwait(false);

		_log.LogInformation("ToggleOn from '{From}' to '{To}'", from, to);
	}

	/// <summary>
	/// Returns the time the animation should take in milliseconds.
	/// </summary>
	private int GetDurationMs(WtqApp app, ToggleModifiers mods)
	{
		switch (mods)
		{
			case ToggleModifiers.Instant:
				return 0;

			case ToggleModifiers.SwitchingApps:
				return 50;

			case ToggleModifiers.None:
			default:
				return 250;
		}
	}

	/// <summary>
	/// Get the position rect a window should move to when toggling OFF.
	/// </summary>
	private WtqRect GetToggleOffToWindowRect(WtqRect from1)
	{
		var to = from1 with
		{
			Y = -from1.Height - 100f,
		};

		return to;
	}

	/// <summary>
	/// Get the position rect a window should move to when toggling ON.
	/// </summary>
	private WtqRect GetToggleOnFromWindowRect(WtqRect to)
	{
		var from = to with
		{
			Y = -to.Height - 100f,
		};
		return from;
	}

	/// <summary>
	/// Returns a rectangle representing the screen we want to toggle the terminal onto.
	/// </summary>
	/// <param name="app">The app for which we're figuring out the screen bounds.</param>
	private WtqRect GetToggleOnToScreenRect(WtqApp app)
	{
		Guard.Against.Null(app);

		var prefMon = app.Options.PreferMonitor ?? _opts.CurrentValue.PreferMonitor;
		var monInd = app.Options.MonitorIndex ?? _opts.CurrentValue.MonitorIndex;

		switch (prefMon)
		{
			case PreferMonitor.AtIndex:
			{
				var screens = _scrInfoProvider.GetScreenRects();

				if (screens.Length > monInd)
				{
					return screens[monInd];
				}

				_log.LogWarning(
					"Option '{OptionName}' was set to {MonitorIndex}, but only {MonitorCount} screens were found, falling back to primary",
					nameof(app.Options.MonitorIndex),
					monInd,
					screens.Length);

				return _scrInfoProvider.GetPrimaryScreenRect();
			}

			case PreferMonitor.Primary:
				return _scrInfoProvider.GetPrimaryScreenRect();

			case PreferMonitor.WithCursor:
				return _scrInfoProvider.GetScreenWithCursor();

			default:
			{
				_log.LogWarning(
					"Unknown value '{OptionValue}' for option '{OptionName}', falling back to primary",
					prefMon,
					nameof(app.Options.PreferMonitor));

				return _scrInfoProvider.GetPrimaryScreenRect();
			}
		}
	}

	/// <summary>
	/// Returns the target bounds of the specified <paramref name="app"/>, within the specified <paramref name="screenBounds"/> when its toggling onto the screen.
	/// </summary>
	private WtqRect GetToggleOnToWindowRect(
		WtqApp app,
		WtqRect screenBounds)
	{
		Guard.Against.Null(app);

		// Calculate terminal size.
		var termWidth = (int)(screenBounds.Width * _opts.CurrentValue.HorizontalScreenCoverageIndexForApp(app.Options));
		var termHeight = (int)(screenBounds.Height * _opts.CurrentValue.VerticalScreenCoverageIndexForApp(app.Options));

		// Calculate horizontal position.
		var x = app.Options.HorizontalAlign switch
		{
			// Left
			HorizontalAlign.Left => screenBounds.X,

			// Right
			HorizontalAlign.Right => screenBounds.X + (screenBounds.Width - termWidth),

			// Center
			_ => screenBounds.X + (int)Math.Ceiling((screenBounds.Width / 2f) - (termWidth / 2f)),
		};

		return new WtqRect()
		{
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			X = x,

			// Y, top of the screen + offset
			Y = screenBounds.Y + (int)_opts.CurrentValue.GetVerticalOffsetForApp(app.Options),

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			Width = termWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			Height = termHeight,
		};
	}
}