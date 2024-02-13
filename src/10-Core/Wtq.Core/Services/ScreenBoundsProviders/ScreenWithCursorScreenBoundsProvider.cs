using Wtq.Core.Data;
using Wtq.Core.Services;

namespace Wtq.Services.ScreenBoundsProviders;

public class ScreenWithCursorScreenBoundsProvider
	(IWtqScreenCoordsProvider screenCoordsProvider)
	: IScreenBoundsProvider
{
	private readonly IWtqScreenCoordsProvider _screenCoordsProvider = screenCoordsProvider
		?? throw new ArgumentNullException(nameof(screenCoordsProvider));

	private readonly ILogger _log = Log.For<ScreenWithCursorScreenBoundsProvider>();

	/// <inheritdoc/>
	public WtqRect GetTargetScreenBounds(WtqApp app)
	{
		switch (app.App.PreferMonitor)
		{
			case Core.Configuration.PreferMonitor.AtIndex:
				{
					var scrs = _screenCoordsProvider.GetScreenRects();

					if (scrs.Length > app.App.MonitorIndex)
					{
						return scrs[app.App.MonitorIndex];
					}

					_log.LogWarning(
						"Option '{OptionName}' was set to {MonitorIndex}, but only {MonitorCount} screens were found",
						nameof(app.App.MonitorIndex),
						app.App.MonitorIndex,
						scrs.Length);

					return _screenCoordsProvider.GetPrimaryScreenRect();
				}

			case Core.Configuration.PreferMonitor.Primary:
				return _screenCoordsProvider.GetPrimaryScreenRect();

			case Core.Configuration.PreferMonitor.WithCursor:
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
				_log.LogWarning("Unknown value '{OptionValue}' for option '{OptionName}'", app.App.PreferMonitor, nameof(app.App.PreferMonitor));
				return _screenCoordsProvider.GetPrimaryScreenRect();
		}
	}
}