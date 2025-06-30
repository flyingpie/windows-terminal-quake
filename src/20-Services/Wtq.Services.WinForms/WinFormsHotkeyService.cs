using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Input;
using Wtq.Services.WinForms.Native;
using KeyModifiers = Wtq.Services.WinForms.Native.KeyModifiers;
using Keys = System.Windows.Forms.Keys;

namespace Wtq.Services.WinForms;

public sealed class WinFormsHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotkeyService>();

	private readonly IWtqBus _bus;

	public WinFormsHotkeyService(IOptionsMonitor<WtqOptions> opts, IWtqBus bus)
	{
		_bus = Guard.Against.Null(bus);

		// Update registrations every time the settings file is reloaded.
		opts.OnChange((_, _) =>
		{
			// Apps
			foreach (var app in opts.CurrentValue.Apps)
			{
				foreach (var hk in app.Hotkeys)
				{
					Register(hk.Sequence);
				}
			}

			// Global
			foreach (var hk in opts.CurrentValue.Hotkeys)
			{
				Register(hk.Sequence);
			}
		});

		HotkeyManager.HotkeyPressed += (s, a) =>
		{
			var keySeq = new KeySequence(a.Modifiers.ToWtqKeyModifiers(), null, a.Key.ToWtqKeys());

			bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};
	}

	private void Register(KeySequence sequence)
	{
		if (!sequence.HasKeyCode)
		{
			_log.LogWarning("Key chars are not supported in the WinForms hotkey registration, please switch to the SharpHook one.");
			return;
		}

		var mods = (KeyModifiers)sequence.Modifiers;
		var key = (Keys)sequence.KeyCode;

		_log.LogInformation("Registering Hotkey '{Sequence}'", sequence);

		HotkeyManager.RegisterHotkey(key, mods);
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}
}