using Wtq.Services.WinForms.Native;

namespace Wtq.Services.WinForms;

public sealed class WinFormsHotkeyService
	: IDisposable, IHostedService
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

	public void Dispose()
	{
		HotkeyManager.Exit();
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		// Only here to make sure an instance of this class is created.
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}