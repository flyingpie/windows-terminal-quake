using Wtq.Data;

namespace Wtq.Services.AppBoundsProviders;

public class MovingAppBoundsProvider(
	IOptionsMonitor<WtqOptions> opts)
	: IAppBoundsProvider
{
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);

	/// <inheritdoc/>
	public WtqRect GetNextAppBounds(
		WtqApp app,
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentAppBounds,
		double progress)
	{
		Guard.Against.Null(app);

		// TODO: Version that moves apps off the bottom?

		// Calculate terminal size.
		var termWidth = (int)(screenBounds.Width * _opts.CurrentValue.HorizontalScreenCoverageIndexForApp(app.Options));
		var termHeight = (int)(screenBounds.Height * _opts.CurrentValue.VerticalScreenCoverageIndexForApp(app.Options));

		// Calculate horizontal position.
		var x = app.Options.HorizontalAlign switch
		{
			// Left
			HorizontalAlign.Left => screenBounds.X,

			// Right
			HorizontalAlign.Right => screenBounds.X + (screenBounds.Width - termWidth),

			// Center
			_ => screenBounds.X + (int)Math.Ceiling((screenBounds.Width / 2f) - (termWidth / 2f)),
		};

		return new WtqRect()
		{
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			X = x,

			// Y, top of the screen + offset
			Y = screenBounds.Y + -screenBounds.Height + (int)Math.Round(screenBounds.Height * progress) + (int)_opts.CurrentValue.GetVerticalOffsetForApp(app.Options),

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			Width = termWidth,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			Height = termHeight,
		};
	}
}