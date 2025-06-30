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

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;

	public WinFormsHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);

		// Update registrations every time the settings file is reloaded.
		opts.OnChange((_, _) => RegisterAll());

		HotkeyManager.HotkeyPressed += (s, a) =>
		{
			var keySeq = new KeySequence(a.Modifiers.ToWtqKeyModifiers(), null, a.Key.ToWtqKeys());

			bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		RegisterAll();

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		HotkeyManager.Exit();

		return ValueTask.CompletedTask;
	}

	private void RegisterAll()
	{
		// Apps
		foreach (var app in _opts.CurrentValue.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				_log.LogInformation("Registering hotkey '{Hotkey}' for app '{App}'", hk.Sequence, app);
				Register(hk.Sequence);
			}
		}

		// Global
		foreach (var hk in _opts.CurrentValue.Hotkeys)
		{
			_log.LogInformation("Registering global hotkey '{Hotkey}'", hk.Sequence);
			Register(hk.Sequence);
		}
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
}