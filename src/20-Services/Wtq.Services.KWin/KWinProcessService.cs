using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinProcessService : IWtqProcessService
{
	private readonly IKWinClient _kwinClient;

	public KWinProcessService(IKWinClient kwinClient)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
	}

	public Task CreateAsync(WtqAppOptions opts)
	{
		// TODO
		return Task.CompletedTask;
	}

	public async Task<WtqWindow?> FindProcessAsync(WtqAppOptions opts)
	{
		try
		{
			var clients = (await _kwinClient.GetClientListAsync(CancellationToken.None).ConfigureAwait(false))
				.Select(c => new KWinWtqWindow(_kwinClient, c))
				.ToList();

			var x = clients.FirstOrDefault(c => c.Matches(opts));

			Console.WriteLine($"GOT {opts.Name}=>{x?.Name}");
			return x;
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