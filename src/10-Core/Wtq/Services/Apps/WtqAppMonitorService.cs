using Microsoft.Extensions.Hosting;

namespace Wtq.Services.Apps;

// TODO: Merge with IWtqAppRepo?
// TODO: Handle configuration changes?
// TODO: Handle modifications to apps on runtime (or just request restart?).
public class WtqAppMonitorService
	: IHostedService
{
	private readonly IWtqAppRepo _apps;
	private readonly IWtqFocusTracker _focusTracker;
	private readonly bool _isRunning = true;
	private readonly ILogger _log = Log.For<WtqAppMonitorService>();
	private readonly IWtqProcessService _procService;

	public WtqAppMonitorService(
		IWtqAppRepo appRepo,
		IWtqFocusTracker focusTracker,
		IWtqProcessService procService)
	{
		_apps = Guard.Against.Null(appRepo);
		_focusTracker = Guard.Against.Null(focusTracker);
		_procService = Guard.Against.Null(procService);
	}

	public void DropFocus()
	{
		if (_focusTracker.LastNonWtqForeground != null)
		{
			var p = Process.GetProcessById((int)_focusTracker.LastNonWtqForeground);
			if (p == null)
			{
				_log.LogWarning("No foreground process found with id '{Id}'", _focusTracker.LastNonWtqForeground);
				return;
			}

			_procService.BringToForeground(p);
		}
	}

	[SuppressMessage("Reliability", "CA2016:Forward the 'CancellationToken' parameter to methods", Justification = "MvdO: We do not want the created task to be cancelled here.")]
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(async () =>
		{
			while (_isRunning)
			{
				var sw = Stopwatch.StartNew();

				_log.LogInformation("Updating app process");

				try
				{
					await _apps.UpdateAppsAsync().ConfigureAwait(false);
					await UpdateAppProcessesAsync().ConfigureAwait(false);

					_log.LogInformation("Updated app process, took {Elapsed}", sw.Elapsed);
				}
				catch (Exception ex)
				{
					_log.LogError(ex, "Error updating list of apps: {Message}", ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
			}
		});

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task UpdateAppProcessesAsync()
	{
		foreach (var app in _apps.Apps)
		{
			await app.UpdateAsync().ConfigureAwait(false);
		}
	}
}