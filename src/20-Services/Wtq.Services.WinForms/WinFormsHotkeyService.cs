using Wtq.Services.WinForms.Native;

namespace Wtq.Services.WinForms;

public sealed class WinFormsHotkeyService : WtqHostedService
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

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		// TODO: Currently necessary to make sure this service is constructed.
		// Maybe remove IHostedService and manually resolve this type on app startup?
		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}
}