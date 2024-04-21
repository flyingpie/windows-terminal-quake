using Wtq.Core.Data;

namespace Wtq.Core.Services.ScreenBoundsProviders;

public interface IScreenBoundsProvider
{
	/// <summary>
	/// Returns a bounding box for the screen where the terminal should be positioned on.
	/// </summary>
	WtqRect GetTargetScreenBounds(WtqApp app);
}