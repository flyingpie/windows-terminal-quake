namespace Wtq.Services;

/// <summary>
/// Calculates rectangles (with both a location and a size) where app windows can be moved to,
/// when toggling on- or off the screen.
/// </summary>
public interface IWtqWindowRectProvider
{
	/// <summary>
	/// Calculates a rectangle that an app window can be moved to, when toggling ON.
	/// </summary>
	Task<Rectangle> GetOnScreenRectAsync(
		Rectangle screenRectDst,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	);

	/// <summary>
	/// Calculates a rectangle that an app window can be moved to, when toggling OFF.<br/>
	/// Returns null if no free location could be found.
	/// </summary>
	Task<Rectangle?> GetOffScreenRectAsync(
		Rectangle screenRectSrc,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	);

	/// <summary>
	/// Calculates rectangles around the specified screen, where an app window could be moved to.<br/>
	/// The rectangles are returned in the order as specified by <see cref="WtqSharedOptions.OffScreenLocations"/>.<br/>
	/// Note that this doesn't yet take into account other screens, these may overlap with.
	/// </summary>
	Rectangle[] GetOffScreenRects(Rectangle screenRect, Rectangle windowRect, WtqAppOptions opts);
}
