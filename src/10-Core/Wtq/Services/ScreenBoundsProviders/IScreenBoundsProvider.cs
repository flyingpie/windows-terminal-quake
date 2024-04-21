using Wtq.Data;

namespace Wtq.Services.ScreenBoundsProviders;

public interface IScreenBoundsProvider
{
	/// <summary>
	/// Returns a bounding box for the screen where the terminal should be positioned on.
	/// </summary>
	WtqRect GetTargetScreenBounds(WtqApp app);
}