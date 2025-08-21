using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
using System.Collections.Concurrent;
using System.IO;
using Wtq.Events;
using Wtq.Services.SharpHook.Input;

namespace Wtq.Services.SharpHook;

/// <summary>
/// Uses SharpHook (https://github.com/TolikPylypchuk/SharpHook) to register hotkeys.
/// </summary>
public class SharpHookHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<SharpHookHotkeyService>();

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;
//	private readonly IWin32KeyService _keyService;

	private readonly SimpleGlobalHook _hook;

	private Task? _hookTask;
	private bool _isSuspended;

	public SharpHookHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);
//		_keyService = Guard.Against.Null(keyService);

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

	protected override async ValueTask OnDisposeAsync()
	{
		_log.LogDebug("Disposing SharpHook");

		// TODO: Seems that some thread still runs even after disposing.
		_hook.Stop();
		_hook.Dispose();

		await _hookTask.NoCtx();
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_hook.KeyTyped += (s, e) =>
		{
			_log.LogWarning("KeyTyped KeyCode:{KeyCode} KeyChar:{KeyChar}", e.Data.KeyCode, e.Data.KeyChar);

			// Make sure we start out _not_ suppressing a key event (seems to stick to previous value sometimes?).
			e.SuppressEvent = false;

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Turn key code into sequence.
			var keySeq = new KeySequence(ToMod(e.RawEvent.Mask), e.Data.KeyChar.ToString(), Wtq.Input.KeyCode.None);

			_log.LogDebug("TYPED: Got key sequence '{Sequence}'", keySeq);

			// Look for a registered hotkey matching the one just pressed.
			var hk = GetHotkeys().FirstOrDefault(h => h.Sequence == keySeq);
			if (hk == null)
			{
				_log.LogDebug("No hotkey mapping found for key sequence '{Sequence}'", keySeq);
				return;
			}

			// Prevent the key from activating other possible actions. Especially useful when the SUPER ("Windows") key was involved.
			e.SuppressEvent = true;

			_log.LogDebug("Got hotkey mapping for key sequence '{Sequence}' (mapping: {Mapping})", keySeq, hk);

			// Send hotkey pressed event for routing.
			_bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};

		_hook.KeyReleased += (s, e) =>
		{
			_log.LogWarning("KeyReleased: {KeyCode}", e.Data.KeyCode);

			var dbg = 2;
		};

		_hook.KeyPressed += (s, e) =>
		{
			_log.LogWarning("KeyPressed: {KeyCode}", e.Data.KeyCode);

			// Make sure we start out _not_ suppressing a key event (seems to stick to previous value sometimes?).
			e.SuppressEvent = false;

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Convert SharpHook key code to WTQ one.
			var keyCode = e.Data.KeyCode.ToWtqKeyCode();

			//// Turn key code into sequence.
			//var keySeq = _keyService.GetKeySequence(keyCode, e.Data.RawCode);
			var keySeq = new KeySequence(ToMod(e.RawEvent.Mask), null, keyCode);

			_log.LogDebug("PRESSED: Got key sequence '{Sequence}'", keySeq);

			// Look for a registered hotkey matching the one just pressed.
			var hk = GetHotkeys().FirstOrDefault(h => h.Sequence == keySeq);
			if (hk == null)
			{
				_log.LogDebug("No hotkey mapping found for key sequence '{Sequence}'", keySeq);
				return;
			}

			// Prevent the key from activating other possible actions. Especially useful when the SUPER ("Windows") key was involved.
			e.SuppressEvent = true;

			_log.LogDebug("Got hotkey mapping for key sequence '{Sequence}' (mapping: {Mapping})", keySeq, hk);

			// Send hotkey pressed event for routing.
			_bus.Publish(new WtqHotkeyPressedEvent(keySeq));
		};

		_hookTask = _hook.RunAsync();

		return Task.CompletedTask;
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

	public KeyModifiers ToMod(EventMask mask)
	{
		switch (mask)
		{
			case EventMask.Alt:
			case EventMask.LeftAlt:
			case EventMask.RightAlt:
				return KeyModifiers.Alt;

			case EventMask.Ctrl:
			case EventMask.LeftCtrl:
			case EventMask.RightCtrl:
				return KeyModifiers.Control;

			case EventMask.Meta:
			case EventMask.LeftMeta:
			case EventMask.RightMeta:
				return KeyModifiers.Super;

			case EventMask.Shift:
			case EventMask.LeftShift:
			case EventMask.RightShift:
				return KeyModifiers.Shift;
				return KeyModifiers.Super;

			default:
				return KeyModifiers.None;
		}
	}
}