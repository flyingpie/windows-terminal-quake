using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading;
using Wtq.Configuration;
using Wtq.Core.Services;

namespace Wtq.Services;

public class WtqAppMonitorService
	: IHostedService
{
	private readonly ILogger _log = Log.For<WtqAppMonitorService>();
	private readonly IOptions<WtqOptions> _opts;
	private readonly IWtqProcessFactory _wtqProcFactory;
	private readonly IWtqProcessService _procService;
	private readonly IWtqFocusTracker _focusTracker;
	private readonly IWtqBus _bus;
	private readonly IWtqAppRepo _apps;

	// TODO: Handle configuration changes?
	//private readonly List<WtqApp> _apps;

	private bool _isRunning = true;

	public WtqAppMonitorService(
		ILogger<WtqAppMonitorService> log,
		IOptions<WtqOptions> opts,
		IWtqAppRepo appRepo,
		IWtqBus bus,
		IWtqFocusTracker focusTracker,
		IWtqProcessService procService,
		IWtqProcessFactory wtqProcFactory)
	{
		_apps = appRepo;
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));
		_opts = opts ?? throw new ArgumentNullException(nameof(opts));
		_focusTracker = focusTracker ?? throw new ArgumentNullException(nameof(focusTracker));
		_wtqProcFactory = wtqProcFactory ?? throw new ArgumentNullException(nameof(wtqProcFactory));

		_procService = procService
		?? throw new ArgumentNullException(nameof(procService));
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

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	private async Task UpdateAppProcessesAsync()
	{
		// TODO: Handle modifications to apps on runtime (or just request restart?).

		var processes = _procService.GetProcesses().OrderBy(p => p.ProcessName).ToList();

		foreach (var app in _apps.Apps)
		{
			await app.UpdateAsync(processes);
		}
	}
}