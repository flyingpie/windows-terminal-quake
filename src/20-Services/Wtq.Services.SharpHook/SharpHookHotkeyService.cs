using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
using Wtq.Events;

namespace Wtq.Services.SharpHook;

/// <summary>
/// Uses SharpHook (https://github.com/TolikPylypchuk/SharpHook) to register hotkeys.
/// </summary>
public class SharpHookHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<SharpHookHotkeyService>();

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;

	private readonly SimpleGlobalHook _hook;
	private bool _isSuspended;

	public SharpHookHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);

		_bus.OnEvent<WtqSuspendHotkeysEvent>(_ =>
		{
			_log.LogInformation("Suspending hotkey events");

			_isSuspended = true;

			return Task.CompletedTask;
		});

		_bus.OnEvent<WtqResumeHotkeysEvent>(_ =>
		{
			_log.LogInformation("Resuming hotkey events");

			_isSuspended = false;

			return Task.CompletedTask;
		});

		// We need the blocking global hook to allow suppressions (i.e. preventing keys from doing other things after triggering WTQ events).
		_hook = new SimpleGlobalHook(GlobalHookType.Keyboard);
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		// Accumulate modifiers (when pressing multiple modifiers, they come in as a separate event each.
		// I.e. pressing CTRL+ALT+1:
		// - Event CTRL Pressed
		// - Event ALT Pressed
		// - Event 1 Pressed
		// So we need to manually combine the modifiers.
		// Maybe look into asking the OS for pressed modifiers directly, but this requires a separate mechanism, next to SharpHook.
		KeyModifiers accMod = KeyModifiers.None;

		_hook.KeyPressed += (s, e) =>
		{
			// Make sure we start out _not_ suppressing a key event (seems to stick to previous value sometimes?).
			e.SuppressEvent = false;

			// Translate SharpHook values to WTQ ones.
			var m = GetModifiers(e.Data.KeyCode);
			var keyCode = (Keys)e.Data.KeyCode;
			var keyChar = User32.KeyCodeToUnicode(e.Data.RawCode);

			// Add to accumulated modifiers.
			accMod |= m;

			var keySeq = new KeySequence(accMod, keyChar, keyCode);

			Console.WriteLine($"VK:{e.Data.KeyCode} SEQ:{keySeq}");

			_log.LogDebug("KeyPressed(modifiers:{Modifiers} ({CumModifiers}), key:{Key})", m, keyCode, accMod);

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Look for a registered hotkey matching the one just pressed.
			var hk = GetHotkeys().FirstOrDefault(h => h.Sequence == keySeq);
			if (hk == null)
			{
				_log.LogDebug("No hotkey mapping found for modifiers '{Modifiers}' and key '{Key}'", accMod, keyCode);
				return;
			}

			e.SuppressEvent = true;

			_bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};

		_hook.KeyReleased += (s, e) =>
		{
			// Make sure we start out _not_ suppressing a key event (seems to stick to previous value sometimes?).
			e.SuppressEvent = false;

			// Translate SharpHook values to WTQ ones.
			var k = (Keys)e.Data.KeyCode;
			var m = GetModifiers(e.Data.KeyCode);

			// Remove from accumulated modifiers.
			accMod ^= m;

			_log.LogDebug("KeyPressed(modifiers:{Modifiers} ({CumModifiers}), key:{Key})", m, k, accMod);
		};

		_ = _hook.RunAsync();

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_log.LogDebug("Disposing SharpHook");

		// TODO: Seems that some thread still runs even after disposing.
		_hook.Dispose();

		return ValueTask.CompletedTask;
	}

	private static KeyModifiers GetModifiers(KeyCode keyCode)
	{
		switch (keyCode)
		{
			case KeyCode.VcLeftAlt:
			case KeyCode.VcRightAlt:
				return KeyModifiers.Alt;

			case KeyCode.VcLeftControl:
			case KeyCode.VcRightControl:
				return KeyModifiers.Control;

			case KeyCode.VcLeftMeta:
			case KeyCode.VcRightMeta:
				return KeyModifiers.Super;

			case KeyCode.VcLeftShift:
			case KeyCode.VcRightShift:
				return KeyModifiers.Shift;

			default:
				return KeyModifiers.None;
		}
	}

	private IEnumerable<HotkeyOptions> GetHotkeys()
	{
		foreach (var hk in _opts.CurrentValue.Hotkeys)
		{
			yield return hk;
		}

		foreach (var app in _opts.CurrentValue.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				yield return hk;
			}
		}
	}
}