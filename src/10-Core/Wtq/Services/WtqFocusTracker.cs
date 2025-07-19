namespace Wtq.Services;

/// <summary>
/// Runs in the background, keeping track of what window has focus.<br/>
/// Fires <see cref="WtqWindowFocusChangedEvent"/> events when focus changes from window to the next.
/// </summary>
public sealed class WtqFocusTracker(
	IWtqBus bus,
	IWtqWindowService windowService)
	: WtqHostedService
{
	private readonly ILogger _log = Log.For<WtqFocusTracker>();

	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly IWtqWindowService _windowService = Guard.Against.Null(windowService);

	private Worker? _loop;
	private WtqWindow? _prev;

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_loop = new(
			nameof(WtqFocusTracker),
			TimeSpan.FromMilliseconds(333),
			async _ =>
			{
				// Get current foreground window (could be null).
				var curr = await _windowService.GetForegroundWindowAsync(cancellationToken).NoCtx();

				// If the window that has focus now, is not the one that had focus last cycle, focus has changed.
				// Note that both the past- and the future window can be null.
				if (_prev != curr)
				{
					_log.LogDebug("Focus went from window '{LostFocus}' to window {GotFocus})", _prev, curr);

					_bus.Publish(new WtqWindowFocusChangedEvent()
					{
						GotFocusWindow = curr,
						LostFocusWindow = _prev,
					});
				}

				// Store for next cycle.
				_prev = curr;
			});

		return Task.CompletedTask;
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await _loop.DisposeAsync().NoCtx();
	}
}