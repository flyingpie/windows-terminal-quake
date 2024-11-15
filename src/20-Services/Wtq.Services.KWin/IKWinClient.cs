using Wtq.Configuration;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

/// <summary>
/// High-level interface to the KWin compositor.
/// </summary>
public interface IKWinClient
{
	Task BringToForegroundAsync(
		KWinWindow window,
		CancellationToken cancellationToken);

	Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken);

	Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken);

	Task<KWinWindow?> GetForegroundWindowAsync(
		CancellationToken cancellationToken);

	Task<KWinWindow?> GetWindowAsync(
		KWinWindow window,
		CancellationToken cancellationToken);

	Task<ICollection<KWinWindow>> GetWindowListAsync(
		CancellationToken cancellationToken);

	Task MoveWindowAsync(
		KWinWindow window,
		Point location,
		CancellationToken cancellationToken);

	Task RegisterHotkeyAsync(
		string name,
		KeyModifiers modifiers,
		Keys key,
		CancellationToken cancellationToken);

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

	Task SetWindowVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken);
}