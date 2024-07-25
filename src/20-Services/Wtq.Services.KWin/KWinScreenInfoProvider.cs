using Wtq.Utils;

namespace Wtq.Services.KWin;

public sealed class KWinScreenInfoProvider : IWtqScreenInfoProvider
{
	private readonly IKWinClient _kwinClient;

	public KWinScreenInfoProvider(IKWinClient kwinClient)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
	}

	public async Task<Rectangle> GetPrimaryScreenRectAsync()
	{
		return (await GetScreenRectsAsync().ConfigureAwait(false))
			.FirstOrDefault();
	}

	public async Task<Rectangle[]> GetScreenRectsAsync()
	{
		var sInfo = await _kwinClient
			.GetSupportInformationAsync(CancellationToken.None)
			.ConfigureAwait(false);

		return sInfo.Screens
			.Select(s => s.Geometry)
			.Select(g => new Rectangle()
			{
				X = g.X,
				Y = g.Y,
				Width = g.Width,
				Height = g.Height,
			})
			.ToArray();
	}

	public async Task<Rectangle> GetScreenWithCursorAsync()
	{
		var cursorPos = await _kwinClient.GetCursorPosAsync(CancellationToken.None).NoCtx();
		var screens = await GetScreenRectsAsync().NoCtx();

		return screens.Any(s => s.Contains(cursorPos))
			? screens.FirstOrDefault(s => s.Contains(cursorPos))
			: await GetPrimaryScreenRectAsync().NoCtx();
	}
}