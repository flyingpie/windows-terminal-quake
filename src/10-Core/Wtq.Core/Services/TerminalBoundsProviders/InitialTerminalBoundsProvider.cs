using Wtq.Core.Data;

namespace Wtq.Services.TerminalBoundsProviders;

/// <summary>
/// Moves the terminal from- and to where the user put it, maintaining both the original position and size.
/// </summary>
public class InitialTerminalBoundsProvider : ITerminalBoundsProvider
{
	private WtqRect _initialBounds;

	public InitialTerminalBoundsProvider(WtqRect initialBounds)
	{
		_initialBounds = initialBounds;
	}

	/// <inheritdoc/>
	public WtqRect GetTerminalBounds(
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentTerminalBounds,
		double progress)
	{
		if (!isOpening && progress >= 1)
		{
			_initialBounds = currentTerminalBounds;
		}

		var res = new WtqRect()
		{
			// Maintain initial X
			X = _initialBounds.X,

			// Move to initial Y
			Y = -currentTerminalBounds.Height + (int)Math.Round(currentTerminalBounds.Height * progress) + _initialBounds.Y,

			// Initial width
			Width = _initialBounds.Width,

			// Initial height
			Height = _initialBounds.Height,
		};

		if (isOpening && progress >= 1)
		{
			_initialBounds = res;
		}

		return res;
	}
}