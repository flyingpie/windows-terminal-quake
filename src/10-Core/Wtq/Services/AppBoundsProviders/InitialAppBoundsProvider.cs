using Wtq.Data;

namespace Wtq.Services.AppBoundsProviders;

/// <summary>
/// Moves the terminal from- and to where the user put it, maintaining both the original position and size.
/// </summary>
public class InitialAppBoundsProvider(
	WtqRect initialBounds)
	: IAppBoundsProvider
{
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
			initialBounds = currentAppBounds;
		}

		var res = new WtqRect()
		{
			// Maintain initial X
			X = initialBounds.X,

			// Move to initial Y
			Y = -currentAppBounds.Height + (int)Math.Round(currentAppBounds.Height * progress) + initialBounds.Y,

			// Initial width
			Width = initialBounds.Width,

			// Initial height
			Height = initialBounds.Height,
		};

		if (isOpening && progress >= 1)
		{
			initialBounds = res;
		}

		return res;
	}
}