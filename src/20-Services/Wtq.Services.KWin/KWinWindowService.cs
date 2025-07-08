using Wtq.Configuration;

namespace Wtq.Services.KWin;

public class KWinWindowService(
	IKWinClient kwinClient)
	: IWtqWindowService
{
	private readonly ILogger _log = Log.For<KWinWindowService>();

	private readonly IKWinClient _kwinClient = Guard.Against.Null(kwinClient);

	/// <summary>
	/// Creates a process start info object for when WTQ has direct access to the host OS, runs as a native (non-sandboxed) app.
	/// </summary>
	public ProcessStartInfo CreateProcessStartInfo(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var startInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName,
			Arguments = opts.Arguments,
			WorkingDirectory = opts.WorkingDirectory,
		};

		// Arguments
		foreach (var arg in opts.ArgumentList)
		{
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			var exp = arg.Argument.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);

			startInfo.ArgumentList.Add(exp);
		}

		return startInfo;
	}

	/// <summary>
	/// Creates a process start info object for when WTQ runs as a Flatpak.
	/// </summary>
	public ProcessStartInfo CreateProcessStartInfoFlatpak(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var startInfo = new ProcessStartInfo()
		{
			FileName = "flatpak-spawn",
		};

		startInfo.ArgumentList.Add("--host");

		// Working directory
		if (!string.IsNullOrWhiteSpace(opts.WorkingDirectory))
		{
			startInfo.ArgumentList.Add("--directory");
			startInfo.ArgumentList.Add(opts.WorkingDirectory);
		}

		// Filename
		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'");
		}

		startInfo.ArgumentList.Add(opts.FileName);

		// Arguments
		foreach (var arg in opts.ArgumentList ?? [])
		{
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			var exp = arg.Argument.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg.Argument, exp);

			startInfo.ArgumentList.Add(exp);
		}

		return startInfo;
	}

	public Task CreateAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		using var process = new Process();

		process.StartInfo = Os.IsFlatpak ? CreateProcessStartInfoFlatpak(opts) : CreateProcessStartInfo(opts);

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

	public async Task<WtqWindow?> GetForegroundWindowAsync(CancellationToken cancellationToken)
	{
		var w = await _kwinClient.GetForegroundWindowAsync(cancellationToken).NoCtx();

		return w != null
			? new KWinWtqWindow(_kwinClient, w)
			: null;
	}

	public List<WtqWindowProperty> GetWindowProperties() =>
	[
#pragma warning disable SA1027 // Use tabs correctly

		new("Filename", w => ((KWinWtqWindow)w).DesktopFileName),
		new("WindowTitle", w => w.WindowTitle),

		new("ResourceClass", w => ((KWinWtqWindow)w).ResourceClass),
		new("ResourceName", w => ((KWinWtqWindow)w).ResourceName),

		new("Location", w => ((KWinWtqWindow)w).FrameGeometry?.Location.ToShortString(), width: 50),
		new("Size", w => ((KWinWtqWindow)w).FrameGeometry?.Size.ToShortString(), width: 50),

		new("Id", w => w.Id, isVisible: false),

#pragma warning restore SA1027 // Use tabs correctly
	];

	public async Task<ICollection<WtqWindow>> GetWindowsAsync(CancellationToken cancellationToken)
	{
		return (await _kwinClient.GetWindowListAsync(cancellationToken).NoCtx())
			.Select(WtqWindow (c) => new KWinWtqWindow(_kwinClient, c))
			.ToList();
	}
}