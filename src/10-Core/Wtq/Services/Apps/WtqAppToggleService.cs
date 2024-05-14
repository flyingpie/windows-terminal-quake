using Wtq.Data;

namespace Wtq.Services.Apps;

public class WtqAppToggleService(
	IOptionsMonitor<WtqOptions> opts,
	IWtqTween tween,
	IWtqScreenInfoProvider scrInfoProvider)
	: IWtqAppToggleService
{
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqTween _tween = Guard.Against.Null(tween);
	private readonly ILogger _log = Log.For<WtqAppToggleService>();
	private readonly IWtqScreenInfoProvider _scrInfoProvider = Guard.Against.Null(scrInfoProvider);

	public async Task ToggleOnAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		app.BringToForeground();

		var durationMs = GetDurationMs(mods);
		var screen = GetTargetScreenBounds(app);

		var to = GetToggleOnRect(app, screen);
		var from = to with
		{
			Y = -to.Height - 100f,
		};

		await _tween.AnimateAsync(from, to, durationMs, AnimationType.EaseOutQuart, app.MoveWindow).ConfigureAwait(false);

		_log.LogInformation("ToggleOn from '{From}' to '{To}'", from, to);
	}

	public async Task ToggleOffAsync(WtqApp app, ToggleModifiers mods)
	{
		Guard.Against.Null(app);

		var durationMs = GetDurationMs(mods);

		var from1 = app.GetWindowRect();
		var to = from1 with
		{
			Y = -from1.Height - 100f,
		};

		_log.LogInformation("ToggleOn from '{From}' to '{To}'", from1, to);

		await _tween.AnimateAsync(from1, to, durationMs, AnimationType.EaseInQuart, app.MoveWindow).ConfigureAwait(false);
	}

	/// <summary>
	/// Returns a rectangle representing the screen we want to toggle the terminal onto.
	/// </summary>
	/// <param name="app">The app for which we're figuring out the screen bounds.</param>
	public WtqRect GetTargetScreenBounds(WtqApp app)
	{
		Guard.Against.Null(app);

		var prefMon = app.Options.PreferMonitor ?? _opts.CurrentValue.PreferMonitor;
		var monInd = app.Options.MonitorIndex ?? _opts.CurrentValue.MonitorIndex;

		switch (prefMon)
		{
			case PreferMonitor.AtIndex:
			{
				var scrs = _scrInfoProvider.GetScreenRects();

				if (scrs.Length > monInd)
				{
					return scrs[monInd];
				}

				_log.LogWarning(
					"Option '{OptionName}' was set to {MonitorIndex}, but only {MonitorCount} screens were found, falling back to primary",
					nameof(app.Options.MonitorIndex),
					monInd,
					scrs.Length);

				return _scrInfoProvider.GetPrimaryScreenRect();
			}

			case PreferMonitor.Primary:
				return _scrInfoProvider.GetPrimaryScreenRect();

			case PreferMonitor.WithCursor:
				return _scrInfoProvider.GetScreenWithCursor();

			default:
				_log.LogWarning(
					"Unknown value '{OptionValue}' for option '{OptionName}'",
					prefMon,
					nameof(app.Options.PreferMonitor));
				return _scrInfoProvider.GetPrimaryScreenRect();
		}
	}

	/// <summary>
	/// Returns the target bounds of the specified <param name="app"/>, when its toggling onto the screen.
	/// </summary>
	/// <param name="app">The app that's being toggle on. Used to fetch its options.</param>
	/// <param name="screenBounds">The screen onto which the app is being toggled, used for alignment.</param>
	/// <param name="progress">How far along we are in the animation, '0' for not started, to '1' for finished.</param>
	public WtqRect GetToggleOnRect(
		WtqApp app,
		WtqRect screenBounds)
	{
		Guard.Against.Null(app);

		// TODO: Version that moves apps off the bottom?

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

	private static int GetDurationMs(ToggleModifiers mods)
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
}