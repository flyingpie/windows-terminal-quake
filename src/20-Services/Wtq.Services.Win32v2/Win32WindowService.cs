namespace Wtq.Services.Win32v2;

public sealed class Win32WindowService(IWin32 win32) :
	IDisposable,
	IWtqWindowService
{
	private readonly ILogger _log = Log.For<Win32WindowService>();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private readonly SemaphoreSlim _lock = new(1);
	private readonly InitLock _initLock = new();

	private readonly IWin32 _win32 = Guard.Against.Null(win32);

	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private ICollection<WtqWindow> _processes = [];

	public async Task CreateAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		await InitAsync().NoCtx();

		await CreateProcessAsync(opts).ConfigureAwait(false);
	}

	public void Dispose()
	{
		_lock.Dispose();
		_initLock.Dispose();
	}

	public async Task<WtqWindow?> FindWindowAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		await InitAsync().NoCtx();

		var allWindows = await GetWindowsAsync(cancellationToken).NoCtx();

		// TODO: Logging

		var matchingWindows = allWindows
			.OfType<Win32WtqWindow>()
			.Where(p => p.Matches(opts))
			.ToList();

		var chosenWindow = matchingWindows.OrderByDescending(w => w.IsMainWindow).FirstOrDefault();

		return chosenWindow;
	}

	public async Task<WtqWindow?> GetForegroundWindowAsync(
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		try
		{
			var fgPid = _win32.GetForegroundProcessId();
			if (fgPid > 0)
			{
				return (await GetWindowsAsync(cancellationToken))
					.Cast<Win32WtqWindow>()
					.FirstOrDefault(w => w.ProcessId == fgPid);
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error looking up foreground process: {Message}", ex.Message);
		}

		return null;
	}

	public List<WtqWindowProperty> GetWindowProperties() =>
	[

#pragma warning disable SA1027 // Use tabs correctly

		new("ProcessName",		w => ((Win32WtqWindow)w).ProcessName),
		new("WindowTitle",		w => w.WindowTitle),
		new("WindowClass",		w => ((Win32WtqWindow)w).WindowClass),

		new("IsMainWindow",		w => ((Win32WtqWindow)w).IsMainWindow,												width: 120),
		new("Location",			w => ((Win32WtqWindow)w).Rect.Location.ToShortString(),								width: 120),
		new("Size",				w => ((Win32WtqWindow)w).Rect.Size.ToShortString(),									width: 120),

		new("ProcessId",		w => ((Win32WtqWindow)w).ProcessId,													width: 120),
		new("ThreadId",			w => ((Win32WtqWindow)w).ThreadId,							isVisible: false,		width: 120),
		new("WindowHandle",		w => ((Win32WtqWindow)w).WindowHandle,						isVisible: false,		width: 120),

#pragma warning restore SA1027 // Use tabs correctly

	];

	public async Task<ICollection<WtqWindow>> GetWindowsAsync(
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		await UpdateProcessesAsync().NoCtx();

		return _processes;
	}

	private async Task CreateProcessAsync(WtqAppOptions opts)
	{
		await InitAsync().NoCtx();

		_log.LogInformation("Creating process for app '{App}'", opts);

		using var process = new Process();

		process.StartInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName,
			Arguments = opts.Arguments,
			WorkingDirectory = opts.WorkingDirectory,

			// If this is set to "false", some apps like PowerShell are spawned within the command prompt,
			// if WTQ runs from the command line.
			//
			// However, it used to be "false" (the default) for a long time, so we may find issues with this change.
			UseShellExecute = true,
		};

		// Arguments
		foreach (var arg in opts.ArgumentList
			.Where(a => !string.IsNullOrWhiteSpace(a.Argument))
			.Select(a => a.Argument!))
		{
			var exp = arg.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);

			process.StartInfo.ArgumentList.Add(exp);
		}

		// Start
		try
		{
			process.Start();
		}
		catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
		{
			throw new WtqException($"Could not start process using file name '{opts.FileName}'. Make sure it exists and the configuration is correct");
		}
		catch (Exception ex)
		{
			throw new WtqException($"Error starting process for app '{opts}': {ex.Message}", ex);
		}

		await UpdateProcessesAsync(force: true).NoCtx();
	}

	private async Task InitAsync()
	{
		await _initLock.InitAsync(() => UpdateProcessesAsync()).NoCtx();
	}

	private async Task UpdateProcessesAsync(bool force = false)
	{
		// TODO: Remove all the locks and use debounce instead?
		if (!force && _nextLookup > DateTimeOffset.UtcNow)
		{
			return;
		}

		try
		{
			await _lock.WaitAsync().NoCtx();

			if (!force && _nextLookup > DateTimeOffset.UtcNow)
			{
				return;
			}

			_log.LogDebug("Looking up list of processes");
			_nextLookup = DateTimeOffset.UtcNow.Add(_lookupInterval);

			_processes = _win32
				.GetWindowList()
				.Where(w => !w.Rect.Size.IsEmpty)
				.Select(w => (WtqWindow)new Win32WtqWindow(_win32, w))
				.ToList();
		}
		finally
		{
			_lock.Release();
		}
	}
}