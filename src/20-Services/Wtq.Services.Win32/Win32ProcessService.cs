using Wtq.Configuration;
using Wtq.Exceptions;
using Wtq.Services.Win32.Extensions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32ProcessService : IHostedService, IWtqProcessService
{
	private readonly ILogger _log = Log.For<Win32ProcessService>();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private IEnumerable<WtqWindow> _processes = [];

	private readonly SemaphoreSlim _lock = new(1);

	public async Task CreateAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		await CreateProcessAsync(opts).ConfigureAwait(false);
	}

	public WtqWindow? FindProcess(WtqAppOptions opts)
	{
		return GetProcesses().FirstOrDefault(p => p.Matches(opts));
	}

	public WtqWindow? GetForegroundWindow()
	{
		try
		{
			var fg = GetForegroundProcessId();
			if (fg > 0)
			{
				return new Win32WtqProcess(Process.GetProcessById((int)fg));
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error looking up foreground process: {Message}", ex.Message);
		}

		return null;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await UpdateProcessesAsync().ConfigureAwait(false);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
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

		var process = new Process()
		{
			StartInfo = new ProcessStartInfo()
			{
				FileName = opts.FileName,
				Arguments = opts.Arguments,
				UseShellExecute = false,
				Environment =
				{
					{
						"WTQ_START", opts.Name
					},
				},
			},
		};

		// Start
		Retry.Default
			.Execute(
				() =>
				{
					try
					{
						process.Start();
					}
					catch (Win32Exception ex) when (ex.Message == "The system cannot find the file specified")
					{
						throw new CancelRetryException($"Could not start process using file name '{opts.FileName}'. Make sure it exists and the configuration is correct");
					}
					catch (Exception ex)
					{
						_log.LogError(ex, "Error starting process: {Message}", ex.Message);
						throw new WtqException($"Error starting instance of app '{opts}': {ex.Message}", ex);
					}
				});

		await UpdateProcessesAsync(force: true).ConfigureAwait(false);
	}

	private IEnumerable<WtqWindow> GetProcesses()
	{
		_ = Task.Run(async () => await UpdateProcessesAsync().ConfigureAwait(false));

		return _processes;
	}

	private async Task UpdateProcessesAsync(bool force = false)
	{
		_log.LogDebug("1 {force}", force);
		// TODO: Remove all the locks and use debounce instead?

		if (!force && _nextLookup > DateTimeOffset.UtcNow)
		{
			return;
		}
		_log.LogDebug("2");
		try
		{
			var sw1 = Stopwatch.StartNew();
			await _lock.WaitAsync().ConfigureAwait(false);
			_log.LogDebug("Waiting for lock took {Elapsed}", sw1.ElapsedMilliseconds);

			if (!force && _nextLookup > DateTimeOffset.UtcNow)
			{
				return;
			}

			_log.LogDebug("Looking up list of processes");
			_nextLookup = DateTimeOffset.UtcNow.Add(_lookupInterval);
			var res = new List<WtqWindow>();
			var sw = Stopwatch.StartNew();
			var procs = Process.GetProcesses();
			_log.LogDebug("Got process list, contained {ProcessCount} processes, took {Elapsed}ms", procs.Length, sw.ElapsedMilliseconds);
			foreach (var proc in procs)
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