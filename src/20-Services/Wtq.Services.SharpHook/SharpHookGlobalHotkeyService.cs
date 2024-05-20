using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Native;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Utils;

namespace Wtq.Services.SharpHook;

public sealed class SharpHookGlobalHotKeyService : IDisposable, IHostedService
{
	private readonly ILogger _log = Log.For<SharpHookGlobalHotKeyService>();
	private readonly SimpleGlobalHook _hook;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	private UioHookEvent? _last;

	public SharpHookGlobalHotKeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus)
	{
		// We only need keyboard events (at the moment), and mouse events cause debug sessions to be really slow.
		_hook = new SimpleGlobalHook(globalHookType: GlobalHookType.Keyboard);

		_bus = bus;
		_opts = opts;

		_hook.KeyPressed += (s, a) =>
		{
			// Ignore repetitions.
			if (_last != null && a.RawEvent.Mask == _last.Value.Mask && a.RawEvent.Keyboard.KeyCode == _last.Value.Keyboard.KeyCode)
			{
				return;
			}

			var mod = a.RawEvent.Mask.ToWtqKeyModifiers();
			var key = a.Data.KeyCode.ToWtqKeys();

			_log.LogDebug("Raw key:[{RawKeyMod}]{RawKey}, mapped key:[{MappedKeyMod}]{MappedKey}", a.RawEvent.Mask, a.Data.KeyCode, mod, key);

			// Only send events if a registration exists for the hit mod+key combo.
			if (!IsRegistered(mod, key))
			{
				return;
			}

			a.SuppressEvent = true;

			_bus.Publish(
				new WtqHotKeyPressedEvent()
				{
					Key = a.Data.KeyCode.ToWtqKeys(), Modifiers = a.RawEvent.Mask.ToWtqKeyModifiers(),
				});

			_last = a.RawEvent;
		};

		_hook.KeyReleased += (s, a) => { _last = null; };
	}

	public void Dispose()
	{
		_hook.Dispose();
	}

	public bool IsRegistered(KeyModifiers keyMods, Keys key)
	{
		return
			_opts.CurrentValue.HotKeys.Any(hk => hk.Modifiers == keyMods && hk.Key == key) ||
			_opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotKey(key, keyMods)) != null;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		new Thread(_hook.Run)
		{
			Name = "SharpHook",
		}.Start();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_hook.Dispose();

		return Task.CompletedTask;
	}
}