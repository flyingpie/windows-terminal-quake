using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.WinForms.Native;
using Wtq.Utils;

namespace Wtq.Services.WinForms;

public class WinFormsHotkeyService : IHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();
	private readonly IWtqBus _bus;

	public WinFormsHotkeyService(IWtqBus bus)
	{
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_bus.OnEvent<WtqRegisterHotkeyEvent>(
			e =>
			{
				var mods = (KeyModifiers)e.Modifiers;
				var key = (Keys)e.Key;

				_log.LogInformation("Registering Hotkey [{Modifiers}] '{Key}'", mods, key);

				HotkeyManager.RegisterHotkey(key, mods);

				return Task.CompletedTask;
			});

		HotkeyManager.HotkeyPressed += (s, a) =>
		{
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