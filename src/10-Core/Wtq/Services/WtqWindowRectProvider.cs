using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Services;

/// <summary>
/// Calculates rectangles (with both a location and a size) where app windows can be moved to,
/// when toggling on- or off the screen.
/// </summary>
public interface IWtqWindowRectProvider
{
	/// <summary>
	/// Calculates a rectangle that an app window can be moved to, when toggling ON.
	/// </summary>
	Task<Rectangle> GetOnScreenRectAsync(
		Rectangle screenRectDst,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	);

	/// <summary>
	/// Calculates a rectangle that an app window can be moved to, when toggling OFF.<br/>
	/// Returns null if no free location could be found.
	/// </summary>
	Task<Rectangle?> GetOffScreenRectAsync(
		Rectangle screenRectSrc,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	);

	/// <summary>
	/// Calculates rectangles around the specified screen, where an app window could be moved to.<br/>
	/// The rectangles are returned in the order as specified by <see cref="WtqSharedOptions.OffScreenLocations"/>.<br/>
	/// Note that this doesn't yet take into account other screens, these may overlap with.
	/// </summary>
	Rectangle[] GetOffScreenRects(
		Rectangle screenRect,
		Rectangle windowRect,
		WtqAppOptions opts
	);
}

/// <inheritdoc cref="IWtqWindowRectProvider"/>
public class WtqWindowRectProvider(IWtqScreenInfoProvider screenInfoProvider) : IWtqWindowRectProvider
{
	private readonly IWtqScreenInfoProvider _screenInfoProvider = Guard.Against.Null(screenInfoProvider);

	/// <inheritdoc/>
	public async Task<Rectangle> GetOnScreenRectAsync(
		Rectangle screenRectDst,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	)
	{
		Guard.Against.Null(opts);

		// Calculate app window size.
		var wnd = opts.GetResize() == Resizing.Always
			? new Rectangle()
			{
				Width = (int)(screenRectDst.Width * opts.GetHorizontalScreenCoverageIndex()),
				Height = (int)(screenRectDst.Height * opts.GetVerticalScreenCoverageIndex()),
			}
			: new Rectangle()
			{
				Width = windowRectSrc.Width,
				Height = windowRectSrc.Height,
			};

		// Calculate horizontal position.
		var x = opts.GetHorizontalAlign() switch
		{
			// Left
			HorizontalAlign.Left => screenRectDst.X,

			// Right
			HorizontalAlign.Right => screenRectDst.X + (screenRectDst.Width - wnd.Width),

			// Center
			_ => screenRectDst.X + (int)Math.Ceiling((screenRectDst.Width / 2f) - (wnd.Width / 2f)),
		};

		return new Rectangle()
		{
			// X, based on the HorizontalAlign and HorizontalScreenCoverage settings
			X = x,

			// Y, top of the screen + offset
			Y = screenRectDst.Y + (int)opts.GetVerticalOffset(),

			// Horizontal Width, based on the width of the screen and HorizontalScreenCoverage
			Width = wnd.Width,

			// Vertical Height, based on the VerticalScreenCoverage, VerticalOffset, and current progress of the animation
			Height = wnd.Height,
		};
	}

	/// <inheritdoc/>
	public async Task<Rectangle?> GetOffScreenRectAsync(
		Rectangle screenRectSrc,
		Rectangle windowRectSrc,
		WtqAppOptions opts
	)
	{
		Guard.Against.Null(opts);

		// All available screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get possible rectangles to move the app to.
		var targetRects = GetOffScreenRects(screenRectSrc, windowRectSrc, opts);

		// Return first target rectangle that does not overlap with a screen.
		var targetRect = targetRects.FirstOrDefault(r => !screenRects.Any(scr => scr.IntersectsWith(r)));

		// Return either the rectangle that we found, or null in case no free screen space is available.
		return !targetRect.IsEmpty ? targetRect : null;
	}

	/// <inheritdoc/>
	public Rectangle[] GetOffScreenRects(
		Rectangle screenRect,
		Rectangle windowRect,
		WtqAppOptions opts
	)
	{
		Guard.Against.Null(opts);
		Guard.Against.Null(windowRect);
		Guard.Against.Null(screenRect);

		// Keep some margin, to prevent the window just peeking onto the screen if things don't line up exactly right.
		var margin = 100;

		return opts
			.GetOffScreenLocations()
			.Select(dir => dir switch
			{
				Above or None => windowRect with
				{
					// Top of the screen, minus height of the app window.
					Y = screenRect.Y - windowRect.Height - margin,
				},

				Below => windowRect with
				{
					// Bottom of the screen.
					Y = screenRect.Y + screenRect.Height + margin,
				},

				Left => windowRect with
				{
					// Left of the screen, minus width of the app window.
					X = screenRect.X - windowRect.Width - margin,
				},

				Right => windowRect with
				{
					// Right of the screen, plus width of the app window.
					X = screenRect.X + screenRect.Width + windowRect.Width + margin,
				},

				_ => throw new WtqException("Unknown toggle direction."),
			})
			.ToArray();
	}
}