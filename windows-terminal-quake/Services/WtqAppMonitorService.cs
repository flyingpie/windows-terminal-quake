using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading;
using Wtq.Configuration;

namespace Wtq.Services;

public class WtqAppMonitorService(
	ILogger<WtqAppMonitorService> log,
	IOptions<WtqOptions> opts)
	: IHostedService
{
	private readonly ILogger<WtqAppMonitorService> _log = log ?? throw new ArgumentNullException(nameof(log));
	private readonly IOptions<WtqOptions> _opts = opts ?? throw new ArgumentNullException(nameof(opts));

	// TODO: Handle configuration changes?
	private readonly List<WtqProcess> _apps = opts.Value.Apps
		.Select(app => new WtqProcess()
		{
			App = app,
		})
		.ToList();

	private bool _isRunning = true;

	public WtqProcess? GetProcessForApp(WtqAppOptions app)
	{
		return _apps.FirstOrDefault(a => a.App == app);
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		_ = Task.Run(async () =>
		{
			while (_isRunning)
			{
				var sw = Stopwatch.StartNew();

				_log.LogInformation("Updating app process");

				try
				{
					await UpdateAppProcessesAsync();

					_log.LogInformation("Updated app process, took {Elapsed}", sw.Elapsed);
				}
				catch (Exception ex)
				{
					_log.LogError(ex, "Error updating list of apps: {Message}", ex.Message);
				}

				await Task.Delay(TimeSpan.FromSeconds(2));
			}
		});
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		foreach(var app in _apps)
		{
			app.Dispose();
		}
	}

	private async Task UpdateAppProcessesAsync()
	{
		// TODO: Handle modifications to apps on runtime (or just request restart?).

		var processes = Process.GetProcesses().OrderBy(p => p.ProcessName).ToList();

		foreach (var app in _apps)
		{
			await app.UpdateAsync(processes);
		}
	}
}