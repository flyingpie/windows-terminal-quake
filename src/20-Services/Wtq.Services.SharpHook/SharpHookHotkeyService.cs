using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Native;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wtq.Events;

namespace Wtq.Services.SharpHook;

public class SharpHookHotkeyService : WtqHostedService
{
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;

	private TaskPoolGlobalHook? _hook;

	public SharpHookHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);

		// TODO
		//_bus.OnEvent<WtqSuspendHotkeysEvent>(async _ => await UnregisterAllAsync(CancellationToken.None));
		//_bus.OnEvent<WtqResumeHotkeysEvent>(async _ => await RegisterAllAsync(CancellationToken.None));
	}

	private IEnumerable<HotkeyOptions> GetHotkeys()
	{
		foreach(var hk in _opts.CurrentValue.Hotkeys)
		{
			yield return hk;
		}

		foreach(var app in _opts.CurrentValue.Apps)
		{
			foreach(var hk in app.Hotkeys)
			{
				yield return hk;
			}
		}
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_hook = new TaskPoolGlobalHook();

		KeyModifiers mod = KeyModifiers.None;

		_hook.KeyPressed += (s, e) =>
		{
			var k = (Keys)e.Data.KeyCode;
			var m = GetModifiers(e.Data.KeyCode);

			mod |= m;

			var hk = GetHotkeys().FirstOrDefault(h => h.Modifiers == mod && h.Key == k);
			if (hk != null)
			{
				_bus.Publish(new WtqHotkeyPressedEvent(mod, k));
			}
		};

		_hook.KeyReleased += (s, e) =>
		{
			var k = (Keys)e.Data.KeyCode;
			var m = GetModifiers(e.Data.KeyCode);

			mod ^= m;
		};

		_ = _hook.RunAsync();

		return Task.CompletedTask;
	}

	private static KeyModifiers GetModifiers(KeyCode keyCode)
	{
		switch(keyCode)
		{
			case KeyCode.VcLeftAlt:
			case KeyCode.VcRightAlt:
				return KeyModifiers.Alt;

			case KeyCode.VcLeftControl:
			case KeyCode.VcRightControl:
				return KeyModifiers.Control;

			case KeyCode.VcLeftMeta:
			case KeyCode.VcRightMeta:
				return KeyModifiers.Super;

			case KeyCode.VcLeftShift:
			case KeyCode.VcRightShift:
				return KeyModifiers.Shift;

			default:
				return KeyModifiers.None;
		}
	}
}