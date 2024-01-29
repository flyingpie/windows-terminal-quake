using Wtq.Core.Data;

namespace Wtq.Services.TerminalBoundsProviders;

/// <summary>
/// Determine the window size & position during a toggle animation.
/// </summary>
public interface ITerminalBoundsProvider
{
	/// <summary>
	/// Determine the window size & position during a toggle animation.
	/// </summary>
	/// <param name="isOpening">
	/// Whether the calling toggler is currently opening (true) or closing (false).
	/// </param>
	/// <param name="screenBounds">
	/// The rectangle in which the terminal should appear/disappear (eg. target screen).
	/// </param>
	/// <param name="currentTerminalBounds">
	/// The bounds of the terminal at the moment the function was called.
	/// </param>
	/// <param name="progress">
	/// Value between 0.0 and 1.0 to indicate the desired position of the window;
	/// at 0.0 the window is completely hidden; at 1.0 it is fully visible/opened.
	/// </param>
	/// </summary>
	/// <returns>
	/// A <see cref="Rectangle"/> representing where the terminal should be positioned.
	/// </returns>
	WtqRect GetTerminalBounds(
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentTerminalBounds,
		double progress);
}