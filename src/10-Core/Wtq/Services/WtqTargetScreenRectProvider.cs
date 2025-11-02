namespace Wtq.Services;

public class WtqTargetScreenRectProvider(IWtqScreenInfoProvider screenInfoProvider) : IWtqTargetScreenRectProvider
{
	private readonly ILogger _log = Log.For<WtqTargetScreenRectProvider>();
	private readonly IWtqScreenInfoProvider _screenInfoProvider = Guard.Against.Null(screenInfoProvider);

	/// <summary>
	/// Returns the rectangle of the screen that should be used to toggle the <paramref name="opts"/> to.
	/// </summary>
	public async Task<Rectangle> GetTargetScreenRectAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		_log.LogTrace("Looking for target screen rect for app {App}", this);

		// Determine what monitor we want to use.
		var preferMonitor = opts.GetPreferMonitor();

		switch (preferMonitor)
		{
			case PreferMonitor.AtIndex:
				{
					// Get configured screen index.
					var screenIndex = opts.GetMonitorIndex();

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
						nameof(opts.MonitorIndex),
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
						nameof(opts.PreferMonitor));

					return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
				}
		}
	}
}