using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Native;
using SharpHook.Providers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq.SharpHook;

public class SharpHookGlobalHotkeyService : IHostedService
{
	private readonly IGlobalHook _hook;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppRepo _appRepo;
	private UioHookEvent? _last;

	// TODO: Make specific to hotkey combinations.
	private List<Func<HotkeyInfo, Task>> _registrations = [];

	public SharpHookGlobalHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus,
		IWtqAppRepo appRepo)
	{
		//_hook = new TaskPoolGlobalHook();
		_hook = new SimpleGlobalHook();

		_bus = bus;
		_opts = opts;
		_appRepo = appRepo;

		_hook.KeyPressed += (s, a) =>
		{
			// Ignore repetitions.
			if (_last != null && a.RawEvent.Mask == _last.Value.Mask && a.RawEvent.Keyboard.KeyCode == _last.Value.Keyboard.KeyCode)
			{
				return;
			}

			var app = GetAppForHotkey(a.RawEvent.Mask.ToWtqKeyModifiers(), a.Data.KeyCode.ToWtqKeys());

			if (a.RawEvent.Mask == ModifierMask.LeftCtrl && a.Data.KeyCode == KeyCode.Vc2)
			{
				a.SuppressEvent = true;
				Console.WriteLine("SUPPRESS");

				// TODO: Put something in between ingesting hotkeys and publishing functional events.
				_bus.Publish(new WtqEvent()
				{
					ActionType = WtqActionType.ToggleApp,
					App = app,
				});

				//var inf = new HotkeyInfo()
				//{
				//	Key = WtqKeys.A,
				//	Modifiers = WtqKeyModifiers.Alt,
				//};

				//foreach (var r in _registrations)
				//{
				//	Task.Run(async () => await r(inf));
				//}
			}

			Console.WriteLine($"KEY PRESSED: [{a.RawEvent.Mask}] {a.Data.KeyCode}");

			_last = a.RawEvent;
		};

		_hook.KeyReleased += (s, a) =>
		{
			_last = null;
		};
	}

	public WtqApp? GetAppForHotkey(WtqKeyModifiers keyMods, WtqKeys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotkey(key, keyMods));
		if (opt == null)
		{
			return null;
		}

		return _appRepo.GetProcessForApp(opt);
	}

	public void OnHotkey(Func<HotkeyInfo, Task> onHotkey)
	{
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		//if (!Debugger.IsAttached)
		{
			_ = _hook.RunAsync();
		}

		var wasAttached = false;
		_ = Task.Run(async () =>
		{
			if (Debugger.IsAttached != wasAttached)
			{
				if (Debugger.IsAttached)
				{
					//_hook.Dispose();
				}
				else
				{
				}

				wasAttached = Debugger.IsAttached;
			}
		});

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine("HK STOPPING");
		_hook.Dispose();
		Console.WriteLine("HK STOPPED");

		return Task.CompletedTask;
	}
}