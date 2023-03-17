using System.Drawing;

namespace WindowsTerminalQuake.TerminalBoundsProviders;

public class MovingTerminalBoundsProvider : ITerminalBoundsProvider
{
	public void OnToggleStart(bool open, Rectangle screenBounds, Rectangle currentTerminalBounds)
	{
	}

	public void OnToggleEnd(bool open, Rectangle screenBounds, Rectangle currentTerminalBounds)
	{
	}

	/// <inheritdoc/>
	public Rectangle GetTerminalBounds(Rectangle screenBounds, Rectangle currentTerminalBounds, double progress)
	{
		var settings = QSettings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

		// Calculate terminal size
		var termWidth = (int)(screenBounds.Width * settings.HorizontalScreenCoverageIndex);
		var termHeight = (int)(screenBounds.Height * settings.VerticalScreenCoverageIndex);

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
			x,

			// Y, top of the screen + offset
			screenBounds.Y + -screenBounds.Height + (int)Math.Round(screenBounds.Height * progress) + settings.VerticalOffset,

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			termWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			termHeight
		);

		Log.Debug($"Target screen bounds: {screenBounds}. Terminal bounds: {res}");

		return res;
	}
}