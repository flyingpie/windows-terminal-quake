// namespace Wtq.Extensions;
//
// public static class WtqScreenInfoExtensions
// {
// 	/// <summary>
// 	/// Returns the rectangle of the screen that the app is on.<br/>
// 	/// Uses the top-left corner of the app window to look for the corresponding screen,
// 	/// which is useful to keep in mind when using multiple screens.
// 	/// </summary>
// 	public async Task<Rectangle> GetScreenRectAsync()
// 	{
// 		_log.LogDebug("Looking for current screen rect for app {App}", this);
//
// 		// Get all screen rects.
// 		var screenRects = await _screenInfoProvider.GetScreenRectsAsync().NoCtx();
//
// 		// Get window rect of this app.
// 		var windowRect = await GetWindowRectAsync().NoCtx();
//
// 		// Look for screen rect that contains the left-top corner of the app window.
// 		// TODO: Use screen with largest overlap instead?
// 		foreach (var screenRect in screenRects)
// 		{
// 			if (screenRect.Contains(windowRect.Location))
// 			{
// 				_log.LogDebug("Got screen {Screen}, for app {App}", screenRect, this);
// 				return screenRect;
// 			}
//
// 			_log.LogDebug("Screen {Screen} does NOT contain app {App}", screenRect, this);
// 		}
//
// 		_log.LogWarning("Could not find screen for app {App} ({Rectangle}), returning primary screen", this, windowRect);
//
// 		return await _screenInfoProvider.GetPrimaryScreenRectAsync().NoCtx();
// 	}
// }