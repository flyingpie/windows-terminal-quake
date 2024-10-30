using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinWindowService(
	IKWinClient kwinClient)
	: IWtqWindowService
{
	private readonly ILogger _log = Log.For<KWinWindowService>();

	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);

	public Task CreateAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		using var process = new Process();

		process.StartInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName,
			Arguments = opts.Arguments,
		};

		process.Start();

		return Task.CompletedTask;
	}

	public async Task<WtqWindow?> FindWindowAsync(WtqAppOptions opts)
	{
		_ = Guard.Against.Null(opts);

		try
		{
			var clients = await GetWindowsAsync().NoCtx();

			return clients.FirstOrDefault(c => c.Matches(opts));
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Failed to look up list of windows: {Message}", ex.Message);
		}

		return null;
	}

	public async Task<WtqWindow?> GetForegroundWindowAsync()
	{
		var w = await _kwinClient.GetForegroundWindowAsync().NoCtx();

		return w != null
			? new KWinWtqWindow(_kwinClient, w)
			: null;
	}

	public async Task<ICollection<WtqWindow>> GetWindowsAsync()
	{
		return (await _kwinClient.GetWindowListAsync(CancellationToken.None).NoCtx())
			.Select(c => (WtqWindow)new KWinWtqWindow(_kwinClient, c))
			.ToList();
	}
}