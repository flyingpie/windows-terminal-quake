namespace Wtq.Services;

/// <summary>
/// TODO: Move this stuff to WtqApp and IWtqProcessService.
/// </summary>
public sealed class WtqProcessFactory : IWtqProcessFactory
{
	private readonly ILogger _log = Log.For<WtqProcessFactory>();

	private readonly IOptions<WtqOptions> _opts;
	private readonly IRetry _retry;
	private readonly IWtqProcessService _procService;

	public WtqProcessFactory(
		IOptions<WtqOptions> opts,
		IRetry retry,
		IWtqProcessService procService)
	{
		_opts = Guard.Against.Null(opts);
		_retry = Guard.Against.Null(retry);
		_procService = Guard.Against.Null(procService);
	}

	public async Task<WtqWindow?> GetProcessAsync(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		switch (opts.AttachMode ?? _opts.Value.AttachMode)
		{
			case AttachMode.Manual:
			{
				_log.LogInformation("Using manual process attach mode for app with options {Options}, skipping process lookup", opts);
				return null;
			}

			case AttachMode.Find:
			{
				_log.LogInformation("Using find-only process attach mode for app with options {Options}, looking for process", opts);

				var process = await _procService.FindWindowAsync(opts).NoCtx();

				if (process != null)
				{
					_log.LogInformation("Got process {Process} for options {Options}", process, opts);
				}
				else
				{
					_log.LogInformation("Got no process for options {Options}", opts);
				}

				return process;
			}

			default:
			case AttachMode.FindOrStart:
			{
				return await _retry
					.ExecuteAsync(
						async () =>
						{
							_log.LogInformation("Using find-or-start process attach mode for app with options {Options}, looking for process", opts);

							var process = await _procService.FindWindowAsync(opts).NoCtx();

							if (process != null)
							{
								_log.LogInformation("Got process {Process} for options {Options}", process, opts);
								return process;
							}

							_log.LogInformation("Got no process for options {Options}, attempting to create one", opts);

							await _procService.CreateAsync(opts).NoCtx();

							throw new WtqException($"Failed to find or start window for app '{opts}'.");
						})
					.NoCtx();
			}
		}
	}
}