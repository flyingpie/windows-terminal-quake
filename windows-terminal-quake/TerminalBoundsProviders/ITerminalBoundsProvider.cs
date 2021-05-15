using System.Drawing;

namespace WindowsTerminalQuake.TerminalBoundsProviders
{
	/// <summary>
	/// Determine the window size & position during a toggle animation.
	/// </summary>
	public interface ITerminalBoundsProvider
	{
		/// <summary>
		/// Determine the window size & position during a toggle animation.
		/// </summary>
		/// <param name="screenBounds">
		/// The rectangle in which the terminal should appear/disappear (eg. target screen).
		/// </param>
		/// <param name="progress">
		/// Value between 0.0 and 1.0 to indicate the desired position of the window;
		/// at 0.0 the window is completely hidden; at 1.0 it is fully visible/opened.
		/// </param>
		/// </summary>
		/// <returns>
		/// A <see cref="Rectangle"/> representing where the terminal should be positioned.
		/// </returns>
		Rectangle GetTerminalBounds(Rectangle screenBounds, double progress);
	}
}