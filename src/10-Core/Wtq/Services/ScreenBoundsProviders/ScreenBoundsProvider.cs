using Wtq.Data;

namespace Wtq.Services.ScreenBoundsProviders;

public class ScreenBoundsProvider(
	IOptionsMonitor<WtqOptions> opts,
	IWtqScreenCoordsProvider screenCoordsProvider)
	: IScreenBoundsProvider
{
	private readonly IOptionsMonitor<WtqOptions> _opts = opts
		?? throw new ArgumentNullException(nameof(opts));

	private readonly IWtqScreenCoordsProvider _screenCoordsProvider = screenCoordsProvider
		?? throw new ArgumentNullException(nameof(screenCoordsProvider));

	private readonly ILogger _log = Log.For<ScreenBoundsProvider>();

	/// <inheritdoc/>
	public WtqRect GetTargetScreenBounds(WtqApp app)
	{
		Guard.Against.Null(app);

		var prefMon = app.Options.PreferMonitor ?? _opts.CurrentValue.PreferMonitor;
		var monInd = app.Options.MonitorIndex ?? _opts.CurrentValue.MonitorIndex;

		switch (prefMon)
		{
			case PreferMonitor.AtIndex:
				{
					var scrs = _screenCoordsProvider.GetScreenRects();

					if (scrs.Length > monInd)
					{
						return scrs[monInd];
					}

					_log.LogWarning(
						"Option '{OptionName}' was set to {MonitorIndex}, but only {MonitorCount} screens were found",
						nameof(app.Options.MonitorIndex),
						monInd,
						scrs.Length);

					return _screenCoordsProvider.GetPrimaryScreenRect();
				}

			case PreferMonitor.Primary:
				return _screenCoordsProvider.GetPrimaryScreenRect();

			case PreferMonitor.WithCursor:
				{
					// TODO: Make nicer.
					_log.LogInformation("Selecting screen with cursor");

					var scrs = _screenCoordsProvider.GetScreenRects();
					var c = _screenCoordsProvider.GetCursorPos();

					return scrs.Any(s => s.Contains(c))
						? scrs.FirstOrDefault(s => s.Contains(c))
						: _screenCoordsProvider.GetPrimaryScreenRect();
				}

			default:
				_log.LogWarning("Unknown value '{OptionValue}' for option '{OptionName}'", prefMon, nameof(app.Options.PreferMonitor));
				return _screenCoordsProvider.GetPrimaryScreenRect();
		}
	}
}