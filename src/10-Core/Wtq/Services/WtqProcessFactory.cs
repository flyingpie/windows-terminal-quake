namespace Wtq.Services;

/// <summary>
/// TODO: Move this stuff to WtqApp and IWtqProcessService.
/// </summary>
public sealed class WtqProcessFactory : IWtqProcessFactory
{
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
				return null;
			}

			case AttachMode.Find:
			{
				return await _procService.FindProcess(opts).ConfigureAwait(false);
			}

			default:
			case AttachMode.FindOrStart:
			{
				return await _retry
					.ExecuteAsync(
						async () =>
						{
							var proc = await _procService.FindProcess(opts).ConfigureAwait(false);

							if (proc != null)
							{
								return proc;
							}

							await _procService.CreateAsync(opts).ConfigureAwait(false);

							throw new WtqException($"Failed to find or start window for app '{opts}'.");
						})
					.ConfigureAwait(false);
			}
		}
	}
}