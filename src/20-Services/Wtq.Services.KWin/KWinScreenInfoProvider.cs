namespace Wtq.Services.KWin;

public sealed class KWinScreenInfoProvider(
	IKWinClient kwinClient)
	: IWtqScreenInfoProvider
{
	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);

	public async Task<Rectangle> GetPrimaryScreenRectAsync()
	{
		return (await GetScreenRectsAsync().NoCtx())
			.FirstOrDefault();
	}

	public async Task<Rectangle[]> GetScreenRectsAsync()
	{
		var sInfo = await _kwinClient
			.GetSupportInformationAsync(CancellationToken.None)
			.NoCtx();

		return sInfo.Screens
			.Select(s => s.Geometry)
			.ToArray();
	}

	public async Task<Rectangle> GetScreenWithCursorAsync()
	{
		var cursorPos = await _kwinClient
			.GetCursorPosAsync(CancellationToken.None)
			.NoCtx();

		var screens = await GetScreenRectsAsync().NoCtx();

		return screens.Any(s => s.Contains(cursorPos))
			? screens.FirstOrDefault(s => s.Contains(cursorPos))
			: await GetPrimaryScreenRectAsync().NoCtx();
	}
}