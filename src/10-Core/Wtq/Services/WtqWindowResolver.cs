namespace Wtq.Services;

/// <inheritdoc cref="IWtqWindowResolver"/>
public sealed class WtqWindowResolver(
	IOptionsMonitor<WtqOptions> opts,
	IWtqWindowService procService)
	: IWtqWindowResolver
{
	private readonly ILogger _log = Log.For<WtqWindowResolver>();

	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqWindowService _windowService = Guard.Against.Null(procService);

	public async Task<WtqWindow?> GetWindowHandleAsync(WtqAppOptions opts, bool allowStartNew)
	{
		Guard.Against.Null(opts);

		var attachMode = _opts.CurrentValue.GetAttachModeForApp(opts);

		switch (attachMode)
		{
			case AttachMode.Manual:
				return await ManualAsync(opts).NoCtx();

			case AttachMode.Find:
				return await FindOrStartAsync(opts, false).NoCtx();

			default:
			case AttachMode.FindOrStart:
			case AttachMode.None:
				return await FindOrStartAsync(opts, allowStartNew).NoCtx();
		}
	}

	private async Task<WtqWindow?> FindOrStartAsync(WtqAppOptions opts, bool allowStartNew)
	{
		_log.LogInformation("Using find-or-start process attach mode for app with options {Options}, looking for process", opts);

		// Look for an existing window first.
		var window1 = await _windowService.FindWindowAsync(opts).NoCtx();
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

		await _windowService.CreateAsync(opts).NoCtx();

		for (var attempt = 0; attempt < 5; attempt++)
		{
			// Look for our newly created window.
			var window2 = await _windowService.FindWindowAsync(opts).NoCtx();
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

	private async Task<WtqWindow?> ManualAsync(WtqAppOptions opts)
	{
		_log.LogInformation("Using manual process attach mode for app with options {Options}, skipping process lookup", opts);

		var window = await _windowService.GetForegroundWindowAsync().NoCtx();

		if (window != null)
		{
			_log.LogTrace("Got foreground window '{Window}' for manual attach", window);
		}
		else
		{
			_log.LogWarning("Cannot manually attach, no foreground window found");
		}

		return window;
	}
}