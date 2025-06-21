using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
using System.Runtime.InteropServices;
using System.Text;
using Wtq.Events;
using System.Windows.Forms;

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
		_hook = new SimpleGlobalHook(
			globalHookType: GlobalHookType.Keyboard);
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
			var keyCode = (Configuration.Keys)e.Data.KeyCode;
			var keyChar = User32X.KeyCodeToUnicode((uint)e.Data.RawCode);

			// Add to accumulated modifiers.
			accMod |= m;

			var keySeq = new KeySequence()
			{
				Modifiers = accMod,
				KeyChar = keyChar,
				KeyCode = keyCode,
			};

			Console.WriteLine($"VK:{e.Data.KeyCode} SEQ:{keySeq}");

			_log.LogDebug("KeyPressed(modifiers:{Modifiers} ({CumModifiers}), key:{Key})", m, keyCode, accMod);

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Look for a registered hotkey matching the one just pressed.
			var hk = GetHotkeys().FirstOrDefault(h => h.Sequence.Equals2(keySeq));
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
			var k = (Configuration.Keys)e.Data.KeyCode;
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

public static class User32X
{
	public static byte VK_LSHIFT =  	0xA0;
	public static byte VK_RSHIFT = 0xA1;
	public static byte VK_LCONTROL = 0xA2;
	public static byte VK_RCONTROL =  	0xA3;
	public static byte VK_LMENU =  	0xA4;
	public static byte VK_RMENU =  	0xA5;

	public static string KeyCodeToUnicode(uint key)
	{
		// TODO: Keyboard layout changes require WTQ restart atm.
		KeysConverter converter = new KeysConverter();
		var res = converter.ConvertToString((System.Windows.Forms.Keys)key);
		Console.WriteLine($"RES:{res}");

		// TODO: When "Control" is pressed, we're not getting back key characters. Kind of understandable, but messes up the registration.
		// Remove "Control" from the keyboard state?
		byte[] keyboardState = new byte[256];
		bool keyboardStateStatus = GetKeyboardState(keyboardState);

		Console.WriteLine($"STATE:{Convert.ToHexString(keyboardState)}");

		if (!keyboardStateStatus)
		{
			return "wups";
		}

		uint virtualKeyCode = (uint)key;
		uint scanCode = MapVirtualKey(virtualKeyCode, 0);
		IntPtr inputLocaleIdentifier = GetKeyboardLayout(0);

		Console.WriteLine($"LAYOUT:{inputLocaleIdentifier}");

		StringBuilder result = new StringBuilder();
		ToUnicodeEx(virtualKeyCode, scanCode, keyboardState, result, (int)5, (uint)0, inputLocaleIdentifier);

		return result.ToString();
	}

	[DllImport("user32.dll")]
	static extern bool GetKeyboardState(byte[] lpKeyState);

	[DllImport("user32.dll")]
	static extern uint MapVirtualKey(uint uCode, uint uMapType);

	[DllImport("user32.dll")]
	static extern IntPtr GetKeyboardLayout(uint idThread);

	[DllImport("user32.dll")]
	static extern int ToUnicodeEx(uint wVirtKey, uint wScanCode, byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, int cchBuff, uint wFlags, IntPtr dwhkl);
}