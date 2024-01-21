namespace Wtq.Services.TerminalBoundsProviders;

public class MovingTerminalBoundsProvider : ITerminalBoundsProvider
{
	/// <inheritdoc/>
	public Rectangle GetTerminalBounds(
		bool isOpening,
		Rectangle screenBounds,
		Rectangle currentTerminalBounds,
		double progress)
	{
		//var settings = QSettings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

		// Calculate terminal size
		var termWidth = (int)(screenBounds.Width * .9);
		var termHeight = (int)(screenBounds.Height * .9);

		// Calculate horizontal position, based on the terminal alignment and the alignment
		var x = screenBounds.X + (int)Math.Ceiling(screenBounds.Width / 2f - termWidth / 2f);

		var res = new Rectangle
		(
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			x,

			// Y, top of the screen + offset
			screenBounds.Y + -screenBounds.Height + (int)Math.Round(screenBounds.Height * progress) + 0,

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			termWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			termHeight
		);

		//Log.Debug($"Target screen bounds: {screenBounds}. Terminal bounds: {res}");

		return res;
	}
}