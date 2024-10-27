namespace Wtq.Services;

public sealed class WtqFocusTracker(
	IWtqBus bus,
	IWtqWindowService windowService)
	: IAsyncInitializable, IAsyncDisposable
{
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly ILogger _log = Log.For<WtqFocusTracker>();
	private readonly IWtqWindowService _windowService = Guard.Against.Null(windowService);

	private bool _isRunning = true;

	private WtqWindow? _prev;

	public Task InitializeAsync()
	{
		// TODO: Generalize loop.
		_ = Task.Run(
			async () =>
			{
				while (_isRunning)
				{
					try
					{
						// Get current foreground window (could be null).
						var curr = await _windowService.GetForegroundWindowAsync().NoCtx();

						// If the window that has focus now, is not the one that had focus last cycle, focus has changed.
						if (_prev != curr)
						{
							_log.LogInformation("Focus went from window '{LostFocus}' to window {GotFocus})", _prev, curr);

							_bus.Publish(
								new WtqWindowFocusChangedEvent()
								{
									GotFocusWindow = curr,
									LostFocusWindow = _prev,
								});
						}

						// Store for next cycle.
						_prev = curr;

						await Task.Delay(TimeSpan.FromMilliseconds(250)).NoCtx();
					}
					catch (Exception ex)
					{
						_log.LogError(ex, "Error tracking focus: {Message}", ex.Message);
					}
				}
			});

		return Task.CompletedTask;
	}

	public ValueTask DisposeAsync()
	{
		_isRunning = false;

		return ValueTask.CompletedTask;
	}
}