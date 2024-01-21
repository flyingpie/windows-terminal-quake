using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using Wtq.Configuration;
using Wtq.Native;
using Wtq.Services;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	IOptions<WtqOptions> opts,
	WtqAppMonitorService appMon,
	Toggler toggler)
	: IHostedService
{
	private readonly ILogger<WtqService> _log = log ?? throw new ArgumentNullException(nameof(log));
	private readonly IOptions<WtqOptions> _opts = opts ?? throw new ArgumentNullException(nameof(opts));

	private readonly WtqAppMonitorService _appMon = appMon ?? throw new ArgumentNullException(nameof(appMon));
	private readonly Toggler _toggler = toggler ?? throw new ArgumentNullException(nameof(toggler));

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Starting");

		HotkeyManager.HotKeyPressed += (s, args) => ToggleStuff(args);

		// Global hotkeys.
		foreach (var hk in _opts.Value.Hotkeys)
		{
			_log.LogInformation("Registering global hotkey '{Hotkey}'", hk);
			HotkeyManager.RegisterHotKey(hk.Key, hk.Modifiers);
		}

		// Per-app hotkeys.
		foreach (var app in _opts.Value.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				_log.LogInformation("Registering hotkey '{Hotkey}' for app '{App}'", hk, app);
				HotkeyManager.RegisterHotKey(hk.Key, hk.Modifiers);
			}
		}

		return Task.CompletedTask;
	}

	private WtqProcess? open = null;
	private WtqProcess? _lastOpen = null;

	private void ToggleStuff(HotKeyEventArgs args)
	{
		_log.LogInformation("Pressed hot key ['{Modifiers}'] + '{HotKey}'", args.Modifiers, args.Key);

		var app = _opts.Value.Apps.FirstOrDefault(a => a.HasHotkey(args.Key, args.Modifiers));

		if (app == null)
		{
			if (open != null)
			{
				_toggler.Toggle(open.Process, false, 200);
				_lastOpen = open;
				open = null;
				return;
			}
			else
			{
				if (_lastOpen == null)
				{
					// TODO
					return;
				}

				open = _lastOpen;
				_toggler.Toggle(open.Process, true, 200);
				return;
			}

			//_log.LogWarning("No app found with assigned hotkeys ['{Modifiers}'] + '{HotKey}'", args.Modifiers, args.Key);
			return;
		}

		var process = _appMon.GetProcessForApp(app);

		if (process == null)
		{
			_log.LogWarning("No process found for app '{App}'", app);
			return;
		}

		if (process.Process == null)
		{
			_log.LogWarning("WTQ process for app '{App}' does not have a process instance assigned", app);
			return;
		}

		if (open != null)
		{
			if (open == process)
			{
				_toggler.Toggle(open.Process, false, 200);
				open = null;
			}
			else
			{
				_toggler.Toggle(open.Process, false, 100);
				_toggler.Toggle(process.Process, true, 100);
				open = process;
			}

			return;
		}

		_log.LogInformation("Toggling process {Process}", process);
		_toggler.Toggle(process.Process, true, 200);

		open = process;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Stopping");

		return Task.CompletedTask;
	}
}