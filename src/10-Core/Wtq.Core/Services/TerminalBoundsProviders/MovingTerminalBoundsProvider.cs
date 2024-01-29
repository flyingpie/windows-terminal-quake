using Wtq.Core.Data;

namespace Wtq.Services.TerminalBoundsProviders;

public class MovingTerminalBoundsProvider : ITerminalBoundsProvider
{
	/// <inheritdoc/>
	public WtqRect GetTerminalBounds(
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentTerminalBounds,
		double progress)
	{
		// TODO: Version that moves apps off the bottom?
		//var settings = QSettings.Instance ?? throw new InvalidOperationException($"Settings.Instance was null");

		// Calculate terminal size
		var termWidth = (int)(screenBounds.Width * .95);
		var termHeight = (int)(screenBounds.Height * .95);

		// Calculate horizontal position, based on the terminal alignment and the alignment
		var x = screenBounds.X + (int)Math.Ceiling(screenBounds.Width / 2f - termWidth / 2f);

		var res = new WtqRect()
		{
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			X = x,

			// Y, top of the screen + offset
			Y = screenBounds.Y + -screenBounds.Height + (int)Math.Round(screenBounds.Height * progress) + 0,

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			Width = termWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			Height = termHeight,
		};

		//Log.Debug($"Target screen bounds: {screenBounds}. Terminal bounds: {res}");

		return res;
	}
}