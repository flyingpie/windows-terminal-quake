using Wtq.Configuration;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

/// <summary>
/// High-level interface to the KWin compositor.
/// </summary>
public interface IKWinClient : IAsyncDisposable
{
	Task BringToForegroundAsync(
		KWinWindow window,
		CancellationToken cancellationToken);

	Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken);

	Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken);

	Task<KWinWindow?> GetForegroundWindowAsync();

	Task<KWinWindow?> GetWindowAsync(
		KWinWindow window);

	Task<ICollection<KWinWindow>> GetWindowListAsync(
		CancellationToken cancellationToken);

	Task MoveWindowAsync(
		KWinWindow window,
		Point location,
		CancellationToken cancellationToken);

	Task RegisterHotkeyAsync(
		string name,
		KeyModifiers modifiers,
		Keys key);

	Task ResizeWindowAsync(
		KWinWindow window,
		Size size,
		CancellationToken cancellationToken);

	Task SetTaskbarIconVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken);

	Task SetWindowAlwaysOnTopAsync(
		KWinWindow window,
		bool isAlwaysOnTop,
		CancellationToken cancellationToken);

	Task SetWindowOpacityAsync(
		KWinWindow window,
		float opacity,
		CancellationToken cancellationToken);
}