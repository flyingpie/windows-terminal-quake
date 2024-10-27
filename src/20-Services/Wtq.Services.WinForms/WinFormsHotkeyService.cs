using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.WinForms.Native;
using Wtq.Utils;

namespace Wtq.Services.WinForms;

public class WinFormsHotkeyService : IHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();

	public WinFormsHotkeyService(IWtqBus bus)
	{
		Guard.Against.Null(bus);

		bus.OnEvent<WtqHotkeyDefinedEvent>(
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
			bus.Publish(new WtqHotkeyPressedEvent()
			{
				Key = a.Key.ToWtqKeys(),
				Modifiers = a.Modifiers.ToWtqKeyModifiers(),
			});
		};
	}

	public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}