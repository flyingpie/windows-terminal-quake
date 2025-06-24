using Wtq.Input;
using Wtq.Services.WinForms.Native;
using KeyModifiers = Wtq.Services.WinForms.Native.KeyModifiers;
using Keys = System.Windows.Forms.Keys;

namespace Wtq.Services.WinForms;

public sealed class WinFormsHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();

	public WinFormsHotkeyService(IWtqBus bus)
	{
		Guard.Against.Null(bus);

		// TODO: Handle suspend/resume
		bus.OnEvent<WtqHotkeyDefinedEvent>(e =>
		{
			if (!e.Sequence.HasKeyCode)
			{
				_log.LogWarning("Key chars are not supported in the WinForms hotkey registration, please switch to the SharpHook one.");
				return Task.CompletedTask;
			}

			var mods = (KeyModifiers)e.Sequence.Modifiers;
			var key = (Keys)e.Sequence.KeyCode;

			_log.LogInformation("Registering Hotkey '{Sequence}'", e.Sequence);

			HotkeyManager.RegisterHotkey(key, mods);

			return Task.CompletedTask;
		});

		HotkeyManager.HotkeyPressed += (s, a) =>
		{
			var keySeq = new KeySequence(a.Modifiers.ToWtqKeyModifiers(), null, a.Key.ToWtqKeys());

			bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}
}