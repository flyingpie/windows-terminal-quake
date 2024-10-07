using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinProcessService(
	IKWinClient kwinClient)
	: IWtqProcessService
{
	private readonly ILogger _log = Log.For<KWinProcessService>();

	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);

	public Task CreateAsync(WtqAppOptions opts)
	{
		// TODO
		return Task.CompletedTask;
	}

	public async Task<WtqWindow?> FindWindowAsync(WtqAppOptions opts)
	{
		try
		{
			var clients = await GetWindowsAsync().NoCtx();

			var x = clients.FirstOrDefault(c => c.Matches(opts));

			Console.WriteLine($"GOT {opts.Name}=>{x?.Name}");
			return x;
		}
		catch (Exception ex)
		{
			var dbg = 2;
			_log.LogError(ex, "Failed to look up list of windows: {Message}", ex.Message);
		}

		return null;
	}

	public WtqWindow? GetForegroundWindow()
	{
		return null;
	}

	public async Task<ICollection<WtqWindow>> GetWindowsAsync()
	{
		return (await _kwinClient.GetWindowListAsync(CancellationToken.None).NoCtx())
			.Select(c => (WtqWindow)new KWinWtqWindow(_kwinClient, c))
			.ToList();
	}
}