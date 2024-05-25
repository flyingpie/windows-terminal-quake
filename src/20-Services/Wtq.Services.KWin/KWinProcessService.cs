using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinProcessService : IWtqProcessService
{
	private readonly IKWinClient _kwinClient;

	public KWinProcessService(IKWinClient kwinClient)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
	}

	public async Task<WtqWindow?> CreateAsync(WtqAppOptions opts)
	{
		return null;
	}

	public async Task<WtqWindow?> FindProcess(WtqAppOptions opts)
	{
		try
		{
			var clients = (await _kwinClient.GetClientListAsync(CancellationToken.None))
				.Select(c => new KWinWtqWindow(_kwinClient, c))
				.ToList();

			return clients.FirstOrDefault(c => c.Matches(opts));
		}
		catch (Exception ex)
		{
			var dbg = 2;
		}

		return null;
	}

	public WtqWindow? GetForegroundWindow()
	{
		return null;
	}
}