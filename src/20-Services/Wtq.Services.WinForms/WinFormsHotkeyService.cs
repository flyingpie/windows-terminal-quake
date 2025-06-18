using Wtq.Configuration;
using Wtq.Services.WinForms.Native;
using KeyModifiers = Wtq.Services.WinForms.Native.KeyModifiers;
using Keys = System.Windows.Forms.Keys;

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
		bus.OnEvent<WtqHotkeyDefinedEvent>(e =>
		{
			if (e.Sequence.KeyCode == null)
			{
				// TODO: Convert code <-> char
				return Task.CompletedTask;
			}

			var mods = (KeyModifiers)e.Sequence.Modifiers;
			var key = (Keys)e.Sequence.KeyCode;

			_log.LogInformation("Registering Hotkey [{Modifiers}] '{Key}'", e.Sequence);

			HotkeyManager.RegisterHotkey(key, mods);

			return Task.CompletedTask;
		});

		HotkeyManager.HotkeyPressed += (s, a) =>
		{
			var keySeq = new KeySequence()
			{
				Modifiers = a.Modifiers.ToWtqKeyModifiers(),

				// KeyChar = "", // TODO: Not supported yet.
				KeyCode = a.Key.ToWtqKeys(),
			};

			bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}
}