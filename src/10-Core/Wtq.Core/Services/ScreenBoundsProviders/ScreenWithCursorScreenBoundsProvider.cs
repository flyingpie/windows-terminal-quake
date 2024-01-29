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
	public WtqRect GetTargetScreenBounds()
	{
		_log.LogInformation($"Selecting screen with cursor.");

		//return Screen.AllScreens
		//	.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position))
		//	?.Bounds
		//	?? Screen.PrimaryScreen.Bounds;

		var scrs = _screenCoordsProvider.GetScreenRects();
		var c = _screenCoordsProvider.GetCursorPos();

		// TODO: Make nicer.
		return scrs.Any(s => s.Contains(c))
			? scrs.FirstOrDefault(s => s.Contains(c))
			: _screenCoordsProvider.GetPrimaryScreenRect();
	}
}