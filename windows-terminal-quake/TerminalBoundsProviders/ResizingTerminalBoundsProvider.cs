using Serilog;
using System;
using System.Drawing;
using WindowsTerminalQuake.Settings;

namespace WindowsTerminalQuake.TerminalBoundsProviders
{
	public class ResizingTerminalBoundsProvider : ITerminalBoundsProvider
	{
		/// <inheritdoc/>
		public Rectangle GetTerminalBounds(Rectangle screenBounds, double progress)
		{
			var settings = QSettings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

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

			Log.Debug($"Target screen bounds: {screenBounds}. Terminal bounds: {res}");

			return res;
		}
	}
}