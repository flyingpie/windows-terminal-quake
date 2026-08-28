namespace Wtq.Services;

/// <inheritdoc cref="IWtqWindowResolver"/>
public sealed class WtqWindowResolver(
	IWtqWindowService procService)
	: IWtqWindowResolver
{
	/// <summary>
	/// How long we keep looking for a window that should appear after we started a process, in <see cref="AttachMode.StartOnly"/> mode.<br/>
	/// Some apps (like Windows Terminal) can take a good number of seconds between process start and window creation.
	/// </summary>
	private static readonly TimeSpan PendingStartTimeout = TimeSpan.FromSeconds(30);

	private readonly ILogger _log = Log.For<WtqWindowResolver>();

	private readonly IWtqWindowService _windowService = Guard.Against.Null(procService);

	/// <summary>
	/// Per app name, keeps track of a process we started, but of which we haven't seen a window yet (see <see cref="AttachMode.StartOnly"/>).
	/// </summary>
	private readonly ConcurrentDictionary<string, PendingStart> _pendingStarts = new(StringComparer.OrdinalIgnoreCase);

	/// <inheritdoc/>
	public async Task<WtqWindow?> GetWindowHandleAsync(WtqAppOptions opts, bool allowStartNew)
	{
		Guard.Against.Null(opts);

		var attachMode = opts.GetAttachMode();

		switch (attachMode)
		{
			case AttachMode.Manual:
				return await ManualAsync(opts).NoCtx();

			case AttachMode.Find:
				return await FindOrStartAsync(opts, false).NoCtx();

			case AttachMode.StartOnly:
				return await StartOnlyAsync(opts, allowStartNew).NoCtx();

			default:
			case AttachMode.FindOrStart:
			case AttachMode.None:
				return await FindOrStartAsync(opts, allowStartNew).NoCtx();
		}
	}

	/// <summary>
	/// Only attaches to windows that appeared as a result of WTQ starting the app, never to pre-existing ones.<br/>
	/// <br/>
	/// Works by taking a snapshot of matching windows right before starting the process, and afterwards only
	/// considering matching windows that were not in that snapshot.
	/// </summary>
	private async Task<WtqWindow?> StartOnlyAsync(WtqAppOptions opts, bool allowStartNew)
	{
		var appName = opts.Name ?? string.Empty;

		// See if we started a process earlier, of which we're still waiting for a window to appear.
		if (_pendingStarts.TryGetValue(appName, out var pending))
		{
			var elapsed = DateTimeOffset.UtcNow - pending.StartedAt;

			if (elapsed > PendingStartTimeout)
			{
				_log.LogWarning("App '{App}' was started {Elapsed} ago, but no new window appeared since, giving up on that start", opts, elapsed);
				_pendingStarts.TryRemove(appName, out _);
			}
			else
			{
				var window = await FindWindowExceptAsync(opts, pending.PreExistingWindowIds, CancellationToken.None).NoCtx();
				if (window != null)
				{
					_log.LogInformation("Got window {Window} for options {Options}, which appeared after we started the app", window, opts);
					_pendingStarts.TryRemove(appName, out _);
					return window;
				}

				// Don't start yet another instance while the previous one is still starting up.
				_log.LogDebug("App '{App}' was started {Elapsed} ago, still waiting for a window to appear", opts, elapsed);
				return null;
			}
		}

		if (!allowStartNew)
		{
			// We're only allowed to attach to windows we started ourselves, so without starting, there's nothing to look for.
			_log.LogDebug("Using start-only attach mode for app with options {Options}, not allowed to start a new instance, so not looking for windows", opts);
			return null;
		}

		// Take note of any matching windows that exist right now, so we can tell them apart from the one we're about to create.
		var preExisting = (await _windowService.FindWindowsAsync(opts, CancellationToken.None).NoCtx())
			.Select(w => w.Id)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		_log.LogInformation("Using start-only attach mode for app with options {Options}, starting new instance (ignoring {Count} pre-existing windows)", opts, preExisting.Count);

		try
		{
			await _windowService.CreateAsync(opts, CancellationToken.None).NoCtx();
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Could not create process for app '{App}': {Message}", opts, ex.Message);
			return null;
		}

		// Remember that we started something, so that subsequent (periodic) calls keep looking for the resulting window.
		_pendingStarts[appName] = new PendingStart(DateTimeOffset.UtcNow, preExisting);

		for (var attempt = 0; attempt < 5; attempt++)
		{
			// Look for our newly created window.
			var window = await FindWindowExceptAsync(opts, preExisting, CancellationToken.None).NoCtx();
			if (window == null)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(250)).NoCtx();
				continue;
			}

			// If we got one, great, return it.
			_log.LogInformation("Got window {Window} for options {Options}", window, opts);
			_pendingStarts.TryRemove(appName, out _);
			return window;
		}

		_log.LogInformation("No new window appeared yet for app '{App}', will keep looking for up to {Timeout}", opts, PendingStartTimeout);
		return null;
	}

	private async Task<WtqWindow?> FindOrStartAsync(WtqAppOptions opts, bool allowStartNew)
	{
		_log.Log(allowStartNew ? LogLevel.Information : LogLevel.Debug, "Using find-or-start process attach mode for app with options {Options}, looking for process (allow start new: {AllowStartNew})", opts, allowStartNew);

		// Look for an existing window first.
		var window1 = await FindWindowAsync(opts, CancellationToken.None).NoCtx();
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

		try
		{
			await _windowService.CreateAsync(opts, CancellationToken.None).NoCtx();
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Could not create process for app '{App}': {Message}", opts, ex.Message);
			return null;
		}

		for (var attempt = 0; attempt < 5; attempt++)
		{
			// Look for our newly created window.
			var window2 = await FindWindowAsync(opts, CancellationToken.None).NoCtx();
			if (window2 == null)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(250)).NoCtx();
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

		var window = await _windowService.GetForegroundWindowAsync(CancellationToken.None).NoCtx();

		if (window != null)
		{
			_log.LogInformation("Got foreground window '{Window}' for manual attach", window);
		}
		else
		{
			_log.LogWarning("Cannot manually attach, no foreground window found");
		}

		return window;
	}

	private async Task<WtqWindow?> FindWindowAsync(WtqAppOptions opts, CancellationToken ct)
	{
		// Find windows that match the criteria as specified by the app options.
		var matchingWindows = await _windowService.FindWindowsAsync(opts, ct).NoCtx();

		// Warn if we found more than 1.
		if (matchingWindows.Count > 1)
		{
			_log.LogWarning("Multiple windows found for app options {AppOptions}, you may want to specify more matching criteria.\nThese windows were found (of which the first is chosen):{Windows}", opts, string.Join(", ", matchingWindows.Select(w => $"\n- {w}")));
		}

		// Return the first one.
		// Note that - if multiple windows are found - the window service should prioritize by whatever metric makes sense for them.
		return matchingWindows.FirstOrDefault();
	}

	/// <summary>
	/// Like <see cref="FindWindowAsync"/>, but ignores windows whose <see cref="WtqWindow.Id"/> is in <paramref name="ignoreWindowIds"/>.
	/// </summary>
	private async Task<WtqWindow?> FindWindowExceptAsync(WtqAppOptions opts, IReadOnlySet<string> ignoreWindowIds, CancellationToken ct)
	{
		var matchingWindows = await _windowService.FindWindowsAsync(opts, ct).NoCtx();

		return matchingWindows.FirstOrDefault(w => !ignoreWindowIds.Contains(w.Id));
	}

	/// <summary>
	/// A process that was started, but that hasn't produced a window yet.
	/// </summary>
	/// <param name="StartedAt">When the process was started.</param>
	/// <param name="PreExistingWindowIds">Ids of matching windows that existed before the process was started, and that should thus be ignored.</param>
	private sealed record PendingStart(DateTimeOffset StartedAt, IReadOnlySet<string> PreExistingWindowIds);
}