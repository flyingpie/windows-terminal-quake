using Wtq.Configuration;
using Wtq.Exceptions;
using Wtq.Services.Win32.Extensions;
using Wtq.Services.Win32.Native;
using Wtq.Utils;

namespace Wtq.Services.Win32;

public sealed class Win32ProcessService : IWtqProcessService
{
	private readonly ILogger _log = Log.For<Win32ProcessService>();
	private readonly TimeSpan _lookupInterval = TimeSpan.FromSeconds(2);
	private readonly object _procLock = new();
	private DateTimeOffset _nextLookup = DateTimeOffset.MinValue;
	private IEnumerable<WtqWindow> _processes = [];

	[SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "MvdO: Handled by Win32WtqProcess.")]
	public async Task<WtqWindow?> CreateAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		return new Win32WtqProcess(await CreateProcessAsync(opts).ConfigureAwait(false));
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

	private static uint GetForegroundProcessId()
	{
		var hwnd = User32.GetForegroundWindow();
		User32.GetWindowThreadProcessId(hwnd, out uint pid);

		return pid;
	}

	private Task<Process> CreateProcessAsync(WtqAppOptions opts)
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
					{ "WTQ_START", opts.Name },
				},
			},
		};

		// Start
		Retry.Default
			.Execute(() =>
			{
				try
				{
					process.Start();
					process.Refresh();
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

		return Task.FromResult(process);
	}

	private IEnumerable<WtqWindow> GetProcesses()
	{
		lock (_procLock)
		{
			if (_nextLookup < DateTimeOffset.UtcNow)
			{
				_log.LogDebug("Looking up list of processes");
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
		}

		return _processes;
	}
}