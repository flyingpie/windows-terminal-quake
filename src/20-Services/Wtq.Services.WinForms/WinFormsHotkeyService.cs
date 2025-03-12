using Wtq.Services.WinForms.Native;

namespace Wtq.Services.WinForms;

public sealed class WinFormsHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();

	// TODO: Implement.
	private bool _isSuspended;

	public WinFormsHotkeyService(IWtqBus bus)
	{
		Guard.Against.Null(bus);

		// TODO: Handle suspend/resume
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
			bus.Publish(new WtqHotkeyPressedEvent(a.Modifiers.ToWtqKeyModifiers(), a.Key.ToWtqKeys()));
		};
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}
}