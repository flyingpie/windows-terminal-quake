using Microsoft.Extensions.Hosting;
using Wtq.Services.Win32.Extensions;
using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public sealed class Win32WindowService :
	IDisposable,
	IHostedService, // TODO: Remove
	IWtqWindowService
{
	private readonly ILogger _log = Log.For<Win32WindowService>();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private readonly SemaphoreSlim _lock = new(1);

	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private ICollection<WtqWindow> _processes = [];

	public async Task CreateAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		await CreateProcessAsync(opts).ConfigureAwait(false);
	}

	public async Task StartAsync(
		CancellationToken cancellationToken)
	{
		await UpdateProcessesAsync().ConfigureAwait(false);
	}

	public Task StopAsync(
		CancellationToken cancellationToken)
		=> Task.CompletedTask;

	public void Dispose()
	{
		_lock.Dispose();
	}

	public async Task<WtqWindow?> FindWindowAsync(
		WtqAppOptions opts,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(opts);

		var processes = await GetWindowsAsync(cancellationToken).NoCtx();

		return processes.FirstOrDefault(p => p.Matches(opts));
	}

	public Task<WtqWindow?> GetForegroundWindowAsync(
		CancellationToken cancellationToken)
	{
		try
		{
			var fg = GetForegroundProcessId();
			if (fg > 0)
			{
				return Task.FromResult<WtqWindow?>(new Win32WtqWindow(Process.GetProcessById((int)fg)));
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error looking up foreground process: {Message}", ex.Message);
		}

		return Task.FromResult<WtqWindow?>(null);
	}

	public async Task<ICollection<WtqWindow>> GetWindowsAsync(
		CancellationToken cancellationToken)
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

				var wtqProcess = new Win32WtqWindow(proc);
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