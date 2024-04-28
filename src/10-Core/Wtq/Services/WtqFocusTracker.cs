using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.Apps;

namespace Wtq.Services;

public sealed class WtqFocusTracker(
	IWtqAppRepo appsRepo,
	IWtqBus bus,
	IWtqProcessService procService)
	: IHostedService, IWtqFocusTracker
{
	private readonly IWtqAppRepo _appsRepo = Guard.Against.Null(appsRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);
	private readonly bool _isRunning = true;
	private readonly ILogger _log = Log.For<WtqFocusTracker>();
	private readonly IWtqProcessService _procService = Guard.Against.Null(procService);

	public WtqApp? ForegroundApp { get; private set; }

	public uint? LastNonWtqForeground { get; private set; }

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

					// var fg = User32.GetForegroundWindow();
					var fgPid = _procService.GetForegroundProcessId();

					var pr = _appsRepo.Apps.FirstOrDefault(a => a.Process?.Id == fgPid);

					if (pr == null)
					{
						LastNonWtqForeground = fgPid;
					}

					// Nothing changed.
					if (pr == ForegroundApp)
					{
						continue;
					}

					// Did not have focus before, got focus now.
					if (ForegroundApp == null && pr != null)
					{
						// App gained focus.
						_log.LogInformation("App '{App}' gained focus", pr);
						ForegroundApp = pr;
						_bus.Publish(new WtqAppFocusEvent()
						{
							App = pr,
							GainedFocus = true,
						});
						continue;
					}

					if (ForegroundApp != null && ForegroundApp != pr)
					{
						if (pr == null)
						{
							// App lost focus.
							_log.LogInformation("App '{App}' lost focus (went to PID {Pid})", ForegroundApp, fgPid);

							_bus.Publish(new WtqAppFocusEvent()
							{
								App = ForegroundApp,
								GainedFocus = false,
							});

							ForegroundApp = null;

							continue;
						}
						else
						{
							_log.LogInformation("Focus moved from app '{AppFrom}' to app '{AppTo}'", ForegroundApp, pr);
							ForegroundApp = pr;
							continue;
						}
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
		return Task.CompletedTask;
	}
}