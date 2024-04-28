using Wtq.Data;

namespace Wtq.Services.AppBoundsProviders;

/// <summary>
/// Moves the terminal from- and to where the user put it, maintaining both the original position and size.
/// </summary>
public class InitialAppBoundsProvider : IAppBoundsProvider
{
	private WtqRect _initialBounds;

	public InitialAppBoundsProvider(WtqRect initialBounds)
	{
		_initialBounds = initialBounds;
	}

	/// <inheritdoc/>
	public WtqRect GetNextAppBounds(
		WtqApp app,
		bool isOpening,
		WtqRect screenBounds,
		WtqRect currentAppBounds,
		double progress)
	{
		if (!isOpening && progress >= 1)
		{
			_initialBounds = currentAppBounds;
		}

		var res = new WtqRect()
		{
			// Maintain initial X
			X = _initialBounds.X,

			// Move to initial Y
			Y = -currentAppBounds.Height + (int)Math.Round(currentAppBounds.Height * progress) + _initialBounds.Y,

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