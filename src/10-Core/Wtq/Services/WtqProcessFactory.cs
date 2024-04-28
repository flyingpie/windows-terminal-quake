using System.ComponentModel;

namespace Wtq.Services;

/// <summary>
/// TODO: Move this stuff to WtqApp and IWtqProcessService.
/// </summary>
public sealed class WtqProcessFactory : IWtqProcessFactory
{
	private readonly ILogger _log = Log.For<WtqProcessFactory>();

	private readonly IOptions<WtqOptions> _opts;
	private readonly IWtqProcessService _procService;

	public WtqProcessFactory(
		IOptions<WtqOptions> opts,
		IWtqProcessService procService)
	{
		_opts = Guard.Against.Null(opts);
		_procService = Guard.Against.Null(procService);
	}

	public async Task<Process?> GetProcessAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		var procs = _procService.GetProcesses();

		switch (opts.AttachMode ?? _opts.Value.AttachMode)
		{
			case AttachMode.Start:
				return procs
					.FirstOrDefault(p => opts.Filter(p, true))
					?? await CreateProcessAsync(opts).ConfigureAwait(false);

			case AttachMode.Manual:
				return null;

			case AttachMode.Find:
				return procs
					.FirstOrDefault(p => opts.Filter(p, false));

			default:
			case AttachMode.FindOrStart:
				return procs
					.FirstOrDefault(p => opts.Filter(p, false))
					?? await CreateProcessAsync(opts).ConfigureAwait(false);
		}
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

		// Wait for main window handle to become available.
		Retry.Default
			.Execute(() =>
			{
				try
				{
					_log.LogInformation("Waiting for process input idle");
					process.Refresh();

					if (process.MainWindowHandle == 0)
					{
						throw new WtqException("Main window handle not available yet");
					}
				}
				catch (Exception ex)
				{
					_log.LogWarning(ex, "Error waiting for process input idle: {Message}", ex.Message);
					throw new WtqException($"Error waiting for app instance: {ex.Message}", ex);
				}
			});

		return Task.FromResult(process);
	}
}