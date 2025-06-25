using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;
using Wtq.Events;
using Wtq.Services.SharpHook.Input;
using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook;

/// <summary>
/// Uses SharpHook (https://github.com/TolikPylypchuk/SharpHook) to register hotkeys.
/// </summary>
public class SharpHookHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<SharpHookHotkeyService>();

	private KeysConverter _converter = new();
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

	protected override ValueTask OnDisposeAsync()
	{
		_log.LogDebug("Disposing SharpHook");

		// TODO: Seems that some thread still runs even after disposing.
		_hook.Dispose();

		return ValueTask.CompletedTask;
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_hook.KeyPressed += (s, e) =>
		{
			// Make sure we start out _not_ suppressing a key event (seems to stick to previous value sometimes?).
			e.SuppressEvent = false;

			// Convert SharpHook key code to WTQ one.
			var keyCode = e.Data.KeyCode.ToWtqKeyCode();

			// Attempt to translate the virtual key code to a key character (may return null).
			var keyChar = Win32.KeyCodeToKeyChar(e.Data.RawCode);

			if (keyChar == null)
			{
				var attr = keyCode.GetAttribute<DisplayAttribute>();

//				keyChar = EnumUtils
//					.GetValues<WKC>()
//					.FirstOrDefault(k => k.Value == keyCode)
//					?.Value.ToString()
////					?? keyCode.ToString();
;
				//keyChar = _converter.ConvertToString((Keys)keyCode);
				//keyChar = keyCode.ToString();

				keyChar = attr?.Name ?? keyCode.ToString();

				Console.WriteLine($"NO KEY CHAR, FALLBACK:{keyChar}");
			}
			else
			{
				Console.WriteLine($"GOT KEY CHAR:{keyChar}");
			}

			var mod = GetModifiers(keyCode);
			var keySeq = new KeySequence(mod, keyChar, keyCode);

			Console.WriteLine($"SEQ:{keySeq}");

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Look for a registered hotkey matching the one just pressed.
			var hk = GetHotkeys().FirstOrDefault(h => h.Sequence == keySeq);
			if (hk == null)
			{
				_log.LogDebug("No hotkey mapping found for key sequence '{Sequence}'", keySeq);
				return;
			}

			// Prevent the key from activating other possible actions. Especially useful when the SUPER ("Windows") key was involved.
			e.SuppressEvent = true;
			Console.WriteLine($"APP:{hk}");
			_log.LogDebug("Got hotkey mapping for key sequence '{Sequence}' (mapping: {Mapping})", keySeq, hk);

			// Send hotkey pressed event for routing.
			_bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};

		_ = _hook.RunAsync();

		return Task.CompletedTask;
	}

	/// <summary>
	/// Returns the set of <see cref="KeyModifiers"/> that are currently active.<br/>
	/// Also includes the <see cref="KeyModifiers.Numpad"/> modifier, if the specified <paramref name="keyCode"/> contains a numpad key.
	/// </summary>
	private static KeyModifiers GetModifiers(WKC keyCode)
	{
		var mod2 = KeyModifiers.None;
		if (Win32.IsAltPressed())
		{
			mod2 |= KeyModifiers.Alt;
		}

		if (Win32.IsControlPressed())
		{
			mod2 |= KeyModifiers.Control;
		}

		if (Win32.IsShiftPressed())
		{
			mod2 |= KeyModifiers.Shift;
		}

		if (Win32.IsSuperPressed())
		{
			mod2 |= KeyModifiers.Super;
		}

		if (keyCode.IsNumpad())
		{
			mod2 |= KeyModifiers.Numpad;
		}

		return mod2;
	}

	/// <summary>
	/// Returns the full list of <see cref="HotkeyOptions"/>, both globally and per app.<br/>
	/// Used to determine whether we should send an event, as we're seeing _all_ key presses.
	/// </summary>
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