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

	Task<IEnumerable<KWinWindow>> GetClientListAsync(
		CancellationToken cancellationToken);

	Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken);

	Task MoveClientAsync(
		KWinWindow window,
		Rectangle rect,
		CancellationToken cancellationToken);

	Task<KWinSupportInformation> GetSupportInformationAsync(
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