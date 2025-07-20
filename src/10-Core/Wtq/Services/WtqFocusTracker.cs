namespace Wtq.Services;

/// <summary>
/// Runs in the background, keeping track of what window has focus.<br/>
/// Fires <see cref="WtqWindowFocusChangedEvent"/> events when focus changes from window to the next.
/// </summary>
public sealed class WtqFocusTracker : WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqFocusTracker>();

	private readonly RecurringTask _loop;

	private WtqWindow? _prev;

	/// <summary>
	/// Runs in the background, keeping track of what window has focus.<br/>
	/// Fires <see cref="WtqWindowFocusChangedEvent"/> events when focus changes from window to the next.
	/// </summary>
	public WtqFocusTracker(
		IWtqBus bus,
		IWtqWindowService windowService)
	{
		_ = Guard.Against.Null(bus);
		_ = Guard.Against.Null(windowService);

		_loop = new(
			nameof(WtqFocusTracker),
			TimeSpan.FromMilliseconds(333),
			async ct =>
			{
				// Get current foreground window (could be null).
				var curr = await windowService.GetForegroundWindowAsync(ct).NoCtx();

				// If the window that has focus now, is not the one that had focus last cycle, focus has changed.
				// Note that both the past- and the future window can be null.
				if (_prev != curr)
				{
					_log.LogDebug("Focus went from window '{LostFocus}' to window {GotFocus})", _prev, curr);

					bus.Publish(
						new WtqWindowFocusChangedEvent()
						{
							GotFocusWindow = curr,
							LostFocusWindow = _prev,
						});
				}

				// Store for next cycle.
				_prev = curr;
			});
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_loop.Start();

		return Task.CompletedTask;
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await _loop.DisposeAsync().NoCtx();
	}
}