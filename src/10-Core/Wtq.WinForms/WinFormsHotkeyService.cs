using Microsoft.Extensions.Hosting;
using Wtq.Core.Events;
using Wtq.Core.Services;
using Wtq.Utils;
using Wtq.WinForms;
using Wtq.WinForms.Native;

namespace Wtq.Win32;

public class WinFormsHotkeyService : IHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();
	private readonly IWtqBus _bus;

	private KeyModifiers? _lastKeyMod;
	private Keys? _lastKey;

	public WinFormsHotkeyService(IWtqBus bus)
	{
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_bus.OnAsync(
			e => e is WtqRegisterHotkeyEvent,
			e =>
			{
				var x = (WtqRegisterHotkeyEvent)e;

				var mods = (KeyModifiers)x.Modifiers;
				var key = (Keys)x.Key;

				_log.LogInformation("Registering hotkey [{Modifiers}] '{Key}'", mods, key);

				HotkeyManager.RegisterHotKey(key, mods);

				return Task.CompletedTask;
			});

		HotkeyManager.HotKeyPressed += (s, a) =>
		{
			if (_lastKeyMod == a.Modifiers && _lastKey == a.Key)
			{
				// TODO: Reset on keyup or something.
				//return;
			}

			_lastKeyMod = a.Modifiers;
			_lastKey = a.Key;

			_bus.Publish(new WtqHotkeyPressedEvent()
			{
				Key = a.Key.ToWtqKeys(),
				Modifiers = a.Modifiers.ToWtqKeyModifiers(),
			});
		};
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}