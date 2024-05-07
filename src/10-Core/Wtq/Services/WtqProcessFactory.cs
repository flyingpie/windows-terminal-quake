namespace Wtq.Services;

/// <summary>
/// TODO: Move this stuff to WtqApp and IWtqProcessService.
/// </summary>
public sealed class WtqProcessFactory : IWtqProcessFactory
{
	private readonly IOptions<WtqOptions> _opts;
	private readonly IWtqProcessService _procService;

	public WtqProcessFactory(
		IOptions<WtqOptions> opts,
		IWtqProcessService procService)
	{
		_opts = Guard.Against.Null(opts);
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
					return _procService.FindProcess(opts);
				}

			default:
			case AttachMode.FindOrStart:
				{
					for (var i = 0; i < 5; i++)
					{
						var proc = _procService.FindProcess(opts);

						if (proc != null)
						{
							return proc;
						}

						await _procService.CreateAsync(opts).ConfigureAwait(false);

						await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
					}

					throw new WtqException($"Failed to find or start window for app '{opts}'.");
				}
		}
	}
}