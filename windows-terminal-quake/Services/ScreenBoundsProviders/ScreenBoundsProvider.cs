using System.Windows.Forms;

namespace Wtq.Services.ScreenBoundsProviders;

public class ScreenBoundsProvider : IScreenBoundsProvider
{
	private readonly ILogger _log = Log.For<ScreenBoundsProvider>();

	/// <inheritdoc/>
	public Rectangle GetTargetScreenBounds()
	{
		_log.LogInformation($"Selecting screen with cursor.");

		return Screen.AllScreens
			.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position))
			?.Bounds
			?? Screen.PrimaryScreen.Bounds;
	}
}