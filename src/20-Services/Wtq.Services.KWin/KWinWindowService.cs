using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinWindowService(IKWinClient kwinClient)
	: IWtqWindowService
{
	private readonly ILogger _log = Log.For<KWinWindowService>();

	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);

	public Task CreateAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		using var process = new Process();

		process.StartInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName, Arguments = opts.Arguments,
		};

		process.Start();

		return Task.CompletedTask;
	}

	public async Task<WtqWindow?> FindWindowAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		_ = Guard.Against.Null(opts);

		try
		{
			var clients = await GetWindowsAsync(cancellationToken).NoCtx();

			return clients.FirstOrDefault(c => c.Matches(opts));
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Failed to look up list of windows: {Message}", ex.Message);
		}

		return null;
	}

	public async Task<WtqWindow?> GetForegroundWindowAsync(CancellationToken cancellationToken)
	{
		var w = await _kwinClient.GetForegroundWindowAsync(cancellationToken).NoCtx();

		return w != null
			? new KWinWtqWindow(_kwinClient, w)
			: null;
	}

	public List<WtqWindowProperty> GetWindowProperties() =>
	[
		new("Filename", w => ((KWinWtqWindow)w).DesktopFileName),
		new("Resource Class", w => ((KWinWtqWindow)w).ResourceClass),
		new("Resource Name", w => ((KWinWtqWindow)w).ResourceName),
		new("Window Title", w => w.WindowTitle),
		new("Frame Geometry", w => ((KWinWtqWindow)w).FrameGeometry),
		new("Id", w => w.Id),
	];

	public async Task<ICollection<WtqWindow>> GetWindowsAsync(CancellationToken cancellationToken)
	{
		return (await _kwinClient.GetWindowListAsync(cancellationToken).NoCtx())
			.Select(WtqWindow (c) => new KWinWtqWindow(_kwinClient, c))
			.ToList();
	}
}