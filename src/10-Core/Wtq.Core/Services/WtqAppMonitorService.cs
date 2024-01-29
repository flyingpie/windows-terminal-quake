using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading;
using Wtq.Configuration;
using Wtq.Core.Events;
using Wtq.Core.Services;

namespace Wtq.Services;

public class WtqAppMonitorService
	: IHostedService
{
	private readonly ILogger _log = Log.For<WtqAppMonitorService>();
	private readonly IOptions<WtqOptions> _opts;
	private readonly IWtqProcessFactory _wtqProcFactory;
	private readonly IWtqProcessService _procService;
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
		IWtqProcessService procService,
		IWtqProcessFactory wtqProcFactory)
	{
		_apps = appRepo;
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));
		_opts = opts ?? throw new ArgumentNullException(nameof(opts));
		_wtqProcFactory = wtqProcFactory ?? throw new ArgumentNullException(nameof(wtqProcFactory));

		_procService = procService
		?? throw new ArgumentNullException(nameof(procService));
	}

	public void DropFocus()
	{
		if (_lastNonWtqFocus != null)
		{
			var p = Process.GetProcessById((int)_lastNonWtqFocus);
			if (p == null)
			{
				_log.LogWarning("No foreground process found with id '{Id}'", _lastNonWtqFocus);
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

		_ = Task.Run(async () =>
		{
			while (_isRunning)
			{
				try
				{
					await Task.Delay(TimeSpan.FromMilliseconds(250));

					//var fg = User32.GetForegroundWindow();
					var fgPid = _procService.GetForegroundProcessId();

					var pr = _apps.Apps.FirstOrDefault(a => a.Process?.Id == fgPid);

					if (pr == null)
					{
						_lastNonWtqFocus = fgPid;
					}

					// Nothing changed.
					if (pr == _foreground)
					{
						continue;
					}

					// Did not have focus before, got focus now.
					if (_foreground == null && pr != null)
					{
						// App gained focus.
						_log.LogInformation("App '{App}' gained focus", pr);
						_foreground = pr;
						_bus.Publish(new WtqAppFocusEvent()
						{
							App = pr,
							GainedFocus = true,
						});
						continue;
					}

					if (_foreground != null && _foreground != pr)
					{
						if (pr == null)
						{
							// App lost focus.
							_log.LogInformation("App '{App}' lost focus", _foreground);

							_bus.Publish(new WtqAppFocusEvent()
							{
								App = _foreground,
								GainedFocus = false,
							});

							_foreground = null;

							continue;
						}
						else
						{
							_log.LogInformation("Focus moved from app '{AppFrom}' to app '{AppTo}'", _foreground, pr);
							_foreground = pr;
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
	}

	private WtqApp? _foreground;
	private uint? _lastNonWtqFocus;

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		foreach (var app in _apps.Apps)
		{
			app.Dispose();
		}
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