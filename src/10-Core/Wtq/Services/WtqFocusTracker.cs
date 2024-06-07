using Microsoft.Extensions.Hosting;
using Wtq.Events;

namespace Wtq.Services;

public sealed class WtqFocusTracker(
	IWtqAppRepo appsRepo,
	IWtqBus bus,
	IWtqProcessService procService)
	: IHostedService, IWtqFocusTracker
{
	private readonly IWtqAppRepo _appsRepo = Guard.Against.Null(appsRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly ILogger _log = Log.For<WtqFocusTracker>();
	private readonly IWtqProcessService _procService = Guard.Against.Null(procService);

	private WtqApp? _foregroundApp;
	private bool _isRunning = true;

	public WtqWindow? LastNonWtqForeground { get; private set; }

	public async Task FocusLastNonWtqAppAsync()
	{
		await (LastNonWtqForeground?.BringToForegroundAsync() ?? Task.CompletedTask).ConfigureAwait(false);
	}

	[SuppressMessage("Reliability", "CA2016:Forward the 'CancellationToken' parameter to methods", Justification = "MvdO: We do not want the task to be cancelled here.")]
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(async () =>
		{
			while (_isRunning)
			{
				try
				{
					await Task.Delay(TimeSpan.FromMilliseconds(250)).ConfigureAwait(false);

					var fgWindow = _procService.GetForegroundWindow();

					var pr = _appsRepo.Apps.FirstOrDefault(a => a.Process == fgWindow);

					if (pr == null)
					{
						LastNonWtqForeground = fgWindow;
					}

					// Nothing changed.
					if (pr == _foregroundApp)
					{
						continue;
					}

					// Did not have focus before, got focus now.
					if (_foregroundApp == null && pr != null)
					{
						// App gained focus.
						_log.LogInformation("App '{App}' gained focus", pr);
						_foregroundApp = pr;
						_bus.Publish(new WtqAppFocusEvent()
						{
							App = pr,
							GainedFocus = true,
						});
						continue;
					}

					if (_foregroundApp == null || _foregroundApp == pr)
					{
						continue;
					}

					if (pr == null)
					{
						// App lost focus.
						_log.LogInformation("App '{App}' lost focus (went to window {Window})", _foregroundApp, fgWindow);

						_bus.Publish(new WtqAppFocusEvent()
						{
							App = _foregroundApp,
							GainedFocus = false,
						});

						_foregroundApp = null;
					}
					else
					{
						_log.LogInformation("Focus moved from app '{AppFrom}' to app '{AppTo}'", _foregroundApp, pr);
						_foregroundApp = pr;
					}
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