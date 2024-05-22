using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinProcessService : IWtqProcessService
{
	public async Task<WtqWindow?> CreateAsync(WtqAppOptions opts)
	{
		return null;
	}

	public WtqWindow? FindProcess(WtqAppOptions opts)
	{
		return null;
	}

	public WtqWindow? GetForegroundWindow()
	{
		return null;
	}
}