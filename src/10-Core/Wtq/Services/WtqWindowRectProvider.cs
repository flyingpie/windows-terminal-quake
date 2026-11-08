using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqWindowRectProvider"/>
public class WtqWindowRectProvider(IWtqScreenInfoProvider screenInfoProvider)
	: IWtqWindowRectProvider
{
	private readonly IWtqScreenInfoProvider _screenInfoProvider = Guard.Against.Null(
		screenInfoProvider
	);

	/// <inheritdoc/>
	public async Task<Rectangle> GetOnScreenRectAsync(
		Rectangle screenRectDst,
		Rectangle windowRectSrc,
		WtqApp app
	)
	{
		Guard.Against.Null(app);

		var opts = app.Options;

		var prevPos = app.GetPrevPosition(screenRectDst);

		static int CenterX(Rectangle screen, Rectangle window) =>
			screen.X + (int)Math.Ceiling((screen.Width / 2f) - (window.Width / 2f));

		static int CenterY(Rectangle screen, Rectangle window) =>
			screen.Y + (int)Math.Ceiling((screen.Height / 2f) - (window.Height / 2f));

		// Calculate app window size.
		var wnd =
			opts.GetResize() == Resizing.Always
				? new Rectangle()
				{
					Width = (int)(screenRectDst.Width * opts.GetHorizontalScreenCoverageIndex()),
					Height = (int)(screenRectDst.Height * opts.GetVerticalScreenCoverageIndex()),
				}
				: new Rectangle() { Width = windowRectSrc.Width, Height = windowRectSrc.Height };

		// Calculate horizontal position.
		var x = opts.GetHorizontalAlign() switch
		{
			// Left
			HorizontalAlign.Left => screenRectDst.X,

			// Right
			HorizontalAlign.Right => screenRectDst.X + (screenRectDst.Width - wnd.Width),

			// None (keep current horizontal position)
			// Use the exact X position from the previous toggle onto the target screen.
			// Fallback to centering.
			HorizontalAlign.None => prevPos?.X ?? CenterX(screenRectDst, wnd),

			// Center (and default)
			_ => CenterX(screenRectDst, wnd),
		};

		// Calculate vertical position.
		var y = opts.GetVerticalAlign() switch
		{
			// Center
			VerticalAlign.Center => CenterY(screenRectDst, wnd) + opts.GetVerticalOffset(),

			// Bottom
			VerticalAlign.Bottom => screenRectDst.Y
				+ (screenRectDst.Height - wnd.Height)
				- opts.GetVerticalOffset(),

			// None (keep current position)
			// Use the exact Y position from the previous toggle onto the target screen.
			// Fallback to centering.
			VerticalAlign.None => prevPos?.Y ?? CenterY(screenRectDst, wnd),

			// Top (and default)
			_ => screenRectDst.Y + opts.GetVerticalOffset(),
		};

		return new Rectangle()
		{
			X = x,
			Y = y,

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
		WtqApp app
	)
	{
		Guard.Against.Null(app);

		// All available screen rects.
		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();

		// Get possible rectangles to move the app to.
		var targetRects = GetOffScreenRects(screenRectSrc, windowRectSrc, app.Options);

		// Return first target rectangle that does not overlap with a screen.
		var targetRect = targetRects.FirstOrDefault(r =>
			!screenRects.Any(scr => scr.IntersectsWith(r))
		);

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

		return opts.GetOffScreenLocations()
			.Select(dir =>
				dir switch
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
				}
			)
			.ToArray();
	}
}
