using Wtq.Configuration;
using Wtq.Exceptions;
using Wtq.Services.Win32.Extensions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32WindowService :
	IAsyncInitializable,
	IDisposable,
	IWtqWindowService
{
	private readonly ILogger _log = Log.For<Win32WindowService>();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private readonly SemaphoreSlim _lock = new(1);

	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private ICollection<WtqWindow> _processes = [];

	public async Task CreateAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		await CreateProcessAsync(opts).ConfigureAwait(false);
	}

	public async Task InitializeAsync()
	{
		await UpdateProcessesAsync().ConfigureAwait(false);
	}

	public void Dispose()
	{
		_lock.Dispose();
	}

	public async Task<WtqWindow?> FindWindowAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var processes = await GetWindowsAsync().NoCtx();

		return processes.FirstOrDefault(p => p.Matches(opts));
	}

	public Task<WtqWindow?> GetForegroundWindowAsync()
	{
		try
		{
			var fg = GetForegroundProcessId();
			if (fg > 0)
			{
				return Task.FromResult<WtqWindow?>(new Win32WtqProcess(Process.GetProcessById((int)fg)));
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error looking up foreground process: {Message}", ex.Message);
		}

		return Task.FromResult<WtqWindow?>(null);
	}

	public async Task<ICollection<WtqWindow>> GetWindowsAsync()
	{
		await UpdateProcessesAsync().NoCtx();

		return _processes;
	}

	private static uint GetForegroundProcessId()
	{
		var hwnd = User32.GetForegroundWindow();
		User32.GetWindowThreadProcessId(hwnd, out uint pid);

		return pid;
	}

	private async Task CreateProcessAsync(WtqAppOptions opts)
	{
		_log.LogInformation("Creating process for app '{App}'", opts);

		using var process = new Process();

		process.StartInfo = new ProcessStartInfo()
		{
			FileName = opts.FileName,
			Arguments = opts.Arguments,
			UseShellExecute = false,
			Environment =
			{
				{ "WTQ_START", opts.Name },
			},
		};

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

	private async Task<WtqWindow?> FindOrStartAsync(WtqAppOptions opts, bool allowStartNew)
	{
		_log.LogInformation("Using find-or-start process attach mode for app with options {Options}, looking for process", opts);

		// Look for an existing window first.
		var window1 = await FindWindowAsync(opts).NoCtx();
		if (window1 != null)
		{
			// If we got one, great, return it.
			_log.LogInformation("Got process {Process} for options {Options}", window1, opts);
			return window1;
		}

		// If we didn't get one, see if we can try to make a new one.
		if (!allowStartNew)
		{
			// If not, return empty-handed.
			return null;
		}

		// Try to start a new process that presumably creates the window we're looking for.
		_log.LogInformation("Got no process for options {Options}, attempting to create one", opts);

		await CreateAsync(opts).NoCtx();

		for (var attempt = 0; attempt < 5; attempt++)
		{
			// Look for our newly created window.
			var window2 = await FindWindowAsync(opts).NoCtx();
			if (window2 == null)
			{
				continue;
			}

			// If we got one, great, return it.
			_log.LogInformation("Got process {Process} for options {Options}", window2, opts);
			return window2;
		}

		return null;
	}

	private Task<WtqWindow?> ManualAsync(WtqAppOptions opts)
	{
		_log.LogInformation("Using manual process attach mode for app with options {Options}, skipping process lookup", opts);

		return Task.FromResult<WtqWindow?>(null);
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

			_log.LogInformation("Looking up list of processes");
			_nextLookup = DateTimeOffset.UtcNow.Add(_lookupInterval);
			var res = new List<WtqWindow>();
			foreach (var proc in Process.GetProcesses())
			{
				if (!proc.TryGetHasExited(out var hasExited) || hasExited)
				{
					continue;
				}

				if (!proc.TryGetMainWindowHandle(out var mainWindowHandle) || mainWindowHandle == 0)
				{
					continue;
				}

				var wtqProcess = new Win32WtqProcess(proc);
				res.Add(wtqProcess);
			}

			_processes = res;
		}
		finally
		{
			_lock.Release();
		}
	}
}