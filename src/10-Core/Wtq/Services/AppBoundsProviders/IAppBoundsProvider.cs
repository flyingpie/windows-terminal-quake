using Wtq.Data;

namespace Wtq.Services.AppBoundsProviders;

/// <summary>
/// Determine the window size &amp; position during a toggle animation.
/// </summary>
public interface IAppBoundsProvider
{
	/// <summary>
	/// Determine the window size &amp; position during a toggle animation.
	/// </summary>
	/// <param name="app">
	/// The app to get the bounds of.
	/// </param>
	/// <param name="isOpening">
	/// Whether the calling toggler is currently opening (true) or closing (false).
	/// </param>
	/// <param name="screenBounds">
	/// The rectangle in which the app should appear/disappear (eg. target screen).
	/// </param>
	/// <param name="currentAppBounds">
	/// The bounds of the app at the moment the function was called.
	/// </param>
	/// <param name="progress">
	/// Value between 0.0 and 1.0 to indicate the desired position of the window;
	/// at 0.0 the window is completely hidden; at 1.0 it is fully visible/opened.
	/// </param>
	/// <returns>
	/// A <see cref="WtqRect"/> representing where the app should be positioned.
	/// </returns>
	WtqRect GetNextAppBounds(
		WtqApp app,
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentAppBounds,
		double progress);
}