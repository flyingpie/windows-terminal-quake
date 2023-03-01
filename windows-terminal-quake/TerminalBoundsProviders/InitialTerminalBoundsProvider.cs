using System.Drawing;

namespace WindowsTerminalQuake.TerminalBoundsProviders;

public class InitialTerminalBoundsProvider : ITerminalBoundsProvider
{
	private Rectangle _initialBounds;

	public InitialTerminalBoundsProvider(Process wtProcess)
	{
		_initialBounds = wtProcess?.GetBounds() ?? throw new ArgumentNullException(nameof(wtProcess));
	}

	public void OnToggleStart(bool open, Rectangle screenBounds, Rectangle currentTerminalBounds)
	{
		OnToggle(open, screenBounds, currentTerminalBounds);
	}

	public void OnToggleEnd(bool open, Rectangle screenBounds, Rectangle currentTerminalBounds)
	{
		OnToggle(open, screenBounds, currentTerminalBounds);
	}

	public void OnToggle(bool open, Rectangle screenBounds, Rectangle currentTerminalBounds)
	{
		if (!open && currentTerminalBounds.Width > 100 && currentTerminalBounds.Height > 100)
		{
			Log.Debug($"OnToggle(open: {open}, screenBounds: {screenBounds}, currentTerminalBounds: {currentTerminalBounds})");

			_initialBounds = currentTerminalBounds;
		}
	}

	/// <inheritdoc/>
	public Rectangle GetTerminalBounds(Rectangle screenBounds, Rectangle currentTerminalBounds, double progress)
	{
		var res = new Rectangle
		(
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			_initialBounds.X,

			// Y, top of the screen + offset
			screenBounds.Y + -screenBounds.Height + (int)Math.Round(screenBounds.Height * progress) + _initialBounds.Y,

			// Horizontal Width
			_initialBounds.Width,

			// Vertical Height
			_initialBounds.Height
		);

		Log.Debug($"Target screen bounds: {screenBounds}. Terminal bounds: {res}");

		return res;
	}
}