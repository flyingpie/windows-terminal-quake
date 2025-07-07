using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinWindowService(
	IKWinClient kwinClient)
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
			FileName = "flatpak-spawn",
//			FileName = opts.FileName,
//			Arguments = opts.Arguments,
			WorkingDirectory = opts.WorkingDirectory,
		};

		var args = new string[] { "flatpak-spawn", "--host", opts.FileName, };

		// Arguments
		foreach (var arg in args)
		{
			var exp = arg.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);

			process.StartInfo.ArgumentList.Add(exp);
		}

		process.Start();

		return Task.CompletedTask;
	}

	public async Task<ICollection<WtqWindow>> FindWindowsAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		_ = Guard.Against.Null(opts);

		try
		{
			var clients = await GetWindowsAsync(cancellationToken).NoCtx();

			return clients
				.Where(c => c.Matches(opts))
				.ToList();
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Failed to look up list of windows: {Message}", ex.Message);
		}

		return null;
	}

	public async Task<WtqWindow?> GetForegroundWindowAsync(
		CancellationToken cancellationToken)
	{
		var w = await _kwinClient.GetForegroundWindowAsync(cancellationToken).NoCtx();

		return w != null
			? new KWinWtqWindow(_kwinClient, w)
			: null;
	}

	public List<WtqWindowProperty> GetWindowProperties() =>
	[

#pragma warning disable SA1027 // Use tabs correctly

		new("Filename",			w => ((KWinWtqWindow)w).DesktopFileName),
		new("WindowTitle",		w => w.WindowTitle),

		new("ResourceClass",	w => ((KWinWtqWindow)w).ResourceClass),
		new("ResourceName",		w => ((KWinWtqWindow)w).ResourceName),

		new("Location",			w => ((KWinWtqWindow)w).FrameGeometry?.Location.ToShortString(),		width: 50),
		new("Size",				w => ((KWinWtqWindow)w).FrameGeometry?.Size.ToShortString(),			width: 50),

		new("Id",				w => w.Id,																isVisible: false),

#pragma warning restore SA1027 // Use tabs correctly

	];

	public async Task<ICollection<WtqWindow>> GetWindowsAsync(CancellationToken cancellationToken)
	{
		return (await _kwinClient.GetWindowListAsync(cancellationToken).NoCtx())
			.Select(WtqWindow (c) => new KWinWtqWindow(_kwinClient, c))
			.ToList();
	}
}