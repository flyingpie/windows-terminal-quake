using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Data;
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

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;
	private readonly IWin32 _win32;

	private readonly SimpleGlobalHook _hook;

	private Task? _hookTask;
	private bool _isSuspended;

	public SharpHookHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus,
		IWin32 win32)
	{
		_opts = Guard.Against.Null(opts);
		_bus = Guard.Against.Null(bus);
		_win32 = Guard.Against.Null(win32);

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

		_hook.Stop();
		_hook.Dispose();

		await _hookTask.NoCtx();
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		_hook.KeyPressed += (s, e) =>
		{
			_log.LogTrace("[KeyPressed] Event:{Event}", e.Describe());

			// If hotkeys are suspended, don't do anything.
			if (_isSuspended)
			{
				return;
			}

			// Convert SharpHook key code to WTQ one.
			var keyCode = e.Data.KeyCode.ToWtqKeyCode();

			// Turn key code into sequence.
			var keySeq = _win32.GetKeySequence(keyCode, e.Data.RawCode);

			_log.LogDebug("[KeyPressed] Got key sequence '{Sequence}'", keySeq);

			if (HandleHotkeyPressed(keySeq))
			{
				// Prevent the key from activating other possible actions.
				// Especially useful when the SUPER ("Windows") key was involved.
				e.SuppressEvent = true;
			}
		};

		_hookTask = _hook.RunAsync();

		return Task.CompletedTask;
	}

	/// <summary>
	/// Returns the full list of <see cref="HotkeyOptions"/>, both globally and per app.<br/>
	/// Used to determine whether we should send an event, as we're seeing _all_ key presses.
	/// </summary>
	private IEnumerable<(HotkeyOptions HotkeyOpts, WtqAppOptions? AppOpts)> GetHotkeys()
	{
		// Global hotkeys
		foreach (var hk in _opts.CurrentValue.Hotkeys)
		{
			yield return (hk, null);
		}

		// Per-app hotkeys
		foreach (var app in _opts.CurrentValue.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				yield return (hk, app);
			}
		}
	}

	private bool HandleHotkeyPressed(KeySequence keySeq)
	{
		// Look for a registered hotkey matching the one just pressed.
		var hk = GetHotkeys().FirstOrDefault(h => h.HotkeyOpts.Sequence == keySeq);
		if (hk.HotkeyOpts == null)
		{
			_log.LogDebug("No hotkey mapping found for key sequence '{Sequence}'", keySeq);
			return false;
		}

		if (hk.AppOpts == null)
		{
			_log.LogDebug("Got global hotkey mapping for key sequence '{Sequence}'", keySeq);
		}
		else
		{
			_log.LogDebug("Got app hotkey mapping for key sequence '{Sequence}' and app '{App}'", keySeq, hk.AppOpts);
		}

		// Send hotkey pressed event for routing.
		_bus.Publish(new WtqHotkeyPressedEvent(keySeq));

		return true;
	}
}