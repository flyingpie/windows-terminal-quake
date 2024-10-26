using Microsoft.Extensions.Hosting;
using Wtq.Events;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqFocusTracker"/>
public sealed class WtqFocusTracker(
	IWtqAppRepo appsRepo,
	IWtqBus bus,
	IWtqWindowService procService)
	: IHostedService, IWtqFocusTracker
{
	// private readonly IWtqAppRepo _appsRepo = Guard.Against.Null(appsRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly ILogger _log = Log.For<WtqFocusTracker>();
	private readonly IWtqWindowService _procService = Guard.Against.Null(procService);

	// private WtqApp? _foregroundApp;
	private bool _isRunning = true;
	// private WtqWindow? _lastNonWtqForeground;

	private WtqWindow? _prev;
	// private WtqWindow? _curr;

	// /// <inheritdoc/>
	// public async Task FocusLastNonWtqAppAsync()
	// {
	// 	await (_lastNonWtqForeground?.BringToForegroundAsync() ?? Task.CompletedTask).NoCtx();
	// }

	[SuppressMessage("Reliability", "CA2016:Forward the 'CancellationToken' parameter to methods", Justification = "MvdO: We do not want the task to be cancelled here.")]
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(async () =>
		{
			while (_isRunning)
			{
				try
				{
					// Get current foreground window (could be null).
					var curr = await _procService.GetForegroundWindowAsync().NoCtx();

					// If the window that has focus now, is not the one that had focus last cycle, focus has changed.
					if (_prev != curr)
					{
						_log.LogInformation("Focus went from window '{LostFocus}' to window {GotFocus})", _prev, curr);

						_bus.Publish(new WtqWindowFocusEvent()
						{
							GotFocusWindow = curr,
							LostFocusWindow = _prev,
						});
					}


					// Store for next cycle.
					_prev = curr;

					await Task.Delay(TimeSpan.FromMilliseconds(250)).NoCtx();


					// if (foregroundApp == null)
					// {
					// 	_lastNonWtqForeground = curr;
					// }

					// // Nothing changed.
					// if (foregroundApp == _foregroundApp)
					// {
					// 	continue;
					// }

					// // Did not have focus before, got focus now.
					// if (_foregroundApp == null && foregroundApp != null)
					// {
					// 	// App gained focus.
					// 	_log.LogInformation("App '{App}' gained focus", foregroundApp);
					// 	_foregroundApp = foregroundApp;
					// 	_bus.Publish(new WtqWindowFocusEvent()
					// 	{
					// 		App = foregroundApp,
					// 		GainedFocus = true,
					// 	});
					// 	continue;
					// }

					// if (_foregroundApp == null || _foregroundApp == foregroundApp)
					// {
					// 	continue;
					// }

					// if (foregroundApp == null)
					// {
					// 	// App lost focus.
					// 	_log.LogInformation("App '{App}' lost focus (went to window {Window})", _foregroundApp, curr);
					//
					// 	_bus.Publish(new WtqWindowFocusEvent()
					// 	{
					// 		App = _foregroundApp,
					// 		GainedFocus = false,
					// 	});
					//
					// 	_foregroundApp = null;
					// }
					// else
					// {
					// 	_log.LogInformation("Focus moved from app '{AppFrom}' to app '{AppTo}'", _foregroundApp, foregroundApp);
					// 	_foregroundApp = foregroundApp;
					// }
				}
				catch (Exception ex)
				{
					_log.LogError(ex, "Error tracking focus: {Message}", ex.Message);
				}
			}
		});

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_isRunning = false;

		return Task.CompletedTask;
	}
}