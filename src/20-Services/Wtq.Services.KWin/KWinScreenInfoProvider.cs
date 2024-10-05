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

		var res = sInfo.Screens
			.Select(s => s.Geometry)
			.ToArray();

		// TODO: Log.

		return res;
	}

	public async Task<Rectangle> GetScreenWithCursorAsync()
	{
		var cursorPos = await _kwinClient
			.GetCursorPosAsync(CancellationToken.None)
			.NoCtx();

		var screens = await GetScreenRectsAsync().NoCtx();

		var res = screens.Any(s => s.Contains(cursorPos))
			? screens.FirstOrDefault(s => s.Contains(cursorPos))
			: await GetPrimaryScreenRectAsync().NoCtx();

		// TODO: Log.

		return res;
	}
}