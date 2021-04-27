using Serilog;
using System;
using System.Drawing;

namespace WindowsTerminalQuake
{
	public interface ITerminalBoundsProvider
	{
		Rectangle GetTerminalBounds(Rectangle screenBounds, double progress);
	}

	public class TerminalBoundsProvider : ITerminalBoundsProvider
	{
		/// <summary>
		/// Determine the window size & position during a toggle animation.
		/// </summary>
		/// <param name="progress">
		/// Value between 0.0 and 1.0 to indicate the desired position of the window;
		/// at 0.0 the window is completely hidden; at 1.0 it is fully visible/opened.
		/// </param>
		/// </summary>
		/// <returns>
		/// A <see cref="Rectangle"/> representing where the terminal should be positioned.
		/// </returns>
		public Rectangle GetTerminalBounds(Rectangle screenBounds, double progress)
		{
			var settings = Settings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

			// Calculate terminal size
			var termWidth = screenBounds.Width * settings.HorizontalScreenCoverageIndex;
			var termHeight = screenBounds.Height * settings.VerticalScreenCoverageIndex;

			// Calculate horizontal position, based on the terminal alignment and the alignment
			var x = settings.HorizontalAlign switch
			{
				// Left
				HorizontalAlign.Left => screenBounds.X,

				// Right
				HorizontalAlign.Right => screenBounds.X + (screenBounds.Width - termWidth),

				// Center
				_ => screenBounds.X + (int)Math.Ceiling(screenBounds.Width / 2f - termWidth / 2f),
			};

			var res = new Rectangle
			(
				// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
				(int)x,

				// Y, top of the screen + offset
				screenBounds.Y + settings.VerticalOffset,

				// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
				(int)termWidth,

				// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
				(int)(termHeight * progress)
			);

			Log.Information($"Terminal bounds: {res}");

			return res;
		}
	}
}