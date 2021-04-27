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

			var scrWidth = screenBounds.Width;
			var horWidthPct = (float)settings.HorizontalScreenCoverage;

			var horWidth = (int)Math.Ceiling(scrWidth / 100f * horWidthPct);
			var x = settings.HorizontalAlign switch
			{
				HorizontalAlign.Left => screenBounds.X,
				HorizontalAlign.Right => screenBounds.X + (screenBounds.Width - horWidth),
				_ => screenBounds.X + (int)Math.Ceiling(scrWidth / 2f - horWidth / 2f),
			};

			screenBounds.Height = (int)Math.Ceiling(screenBounds.Height * settings.VerticalScreenCoverageIndex);
			screenBounds.Height += settings.VerticalOffset;

			return new Rectangle
			(
				// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
				x,

				// Y, always top of the screen
				screenBounds.Y,

				// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
				horWidth,

				// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
				(int)(screenBounds.Height * progress * settings.VerticalScreenCoverageIndex) + settings.VerticalOffset
			);
		}
	}
}