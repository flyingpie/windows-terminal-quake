using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

/// <summary>
/// High-level interface to the KWin compositor.
/// </summary>
public interface IKWinClient //: IAsyncDisposable
{
	Task BringToForegroundAsync(
		KWinWindow window,
		CancellationToken cancellationToken);

	Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken);

	// Task<KWinSupportInformation> GetSupportInformationAsync(
	// 	CancellationToken cancellationToken);

	Task<ICollection<KWinWindow>> GetWindowListAsync(
		CancellationToken cancellationToken);

	Task MoveWindowAsync(
		KWinWindow window,
		Rectangle rect,
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

	Task StartAsync() => Task.CompletedTask;

	// public ValueTask DisposeAsync()
	// {
	// 	return ValueTask.CompletedTask;
	// }
}