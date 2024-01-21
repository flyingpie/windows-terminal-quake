namespace Wtq.Services.TerminalBoundsProviders;

/// <summary>
/// Moves the terminal from- and to where the user put it, maintaining both the original position and size.
/// </summary>
public class InitialTerminalBoundsProvider : ITerminalBoundsProvider
{
	private Rectangle _initialBounds;

	public InitialTerminalBoundsProvider(Rectangle initialBounds)
	{
		_initialBounds = initialBounds;
	}

	/// <inheritdoc/>
	public Rectangle GetTerminalBounds(
		bool isOpening,
		Rectangle screenBounds,
		Rectangle currentTerminalBounds,
		double progress)
	{
		if (!isOpening && progress >= 1)
		{
			_initialBounds = currentTerminalBounds;
		}

		var res = new Rectangle
		(
			// Maintain initial X
			_initialBounds.X,

			// Move to initial Y
			-currentTerminalBounds.Height + (int)Math.Round(currentTerminalBounds.Height * progress) + _initialBounds.Y,

			// Initial width
			_initialBounds.Width,

			// Initial height
			_initialBounds.Height
		);

		//Log.Debug("Progress: {Progress} Target screen bounds: {ScreenBounds}. Terminal bounds: {TerminalBounds}", progress, screenBounds, res);

		if (isOpening && progress >= 1)
		{
			_initialBounds = res;
		}

		return res;
	}
}