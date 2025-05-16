using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Native;
using Wtq.Events;

namespace Wtq.Services.SharpHook;

public class SharpHookHotkeyService : WtqHostedService
{
	private readonly ILogger _log = Log.For<SharpHookHotkeyService>();

	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;

	private SimpleGlobalHook? _hook;
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
	}

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		//_hook = new TaskPoolGlobalHook();
		_hook = new SimpleGlobalHook(); // We need the blocking global hook to allow suppressions.

		KeyModifiers mod = KeyModifiers.None;

		bool x = false;

		Keys suppress1 = Keys.None;
		KeyModifiers suppress2 = KeyModifiers.None;

		_hook.KeyPressed += (s, e) =>
		{
			e.SuppressEvent = false;

			var k = (Keys)e.Data.KeyCode;
			var m = GetModifiers(e.Data.KeyCode);

			mod |= m;

			_log.LogDebug("KeyPressed(modifiers:{Modifiers} ({CumModifiers}), key:{Key})", m, k, mod);

			if (_isSuspended)
			{
				return;
			}

			var hk = GetHotkeys().FirstOrDefault(h => h.Modifiers == mod && h.Key == k);
			if (hk == null)
			{
				_log.LogDebug("No hotkey mapping found for modifiers '{Modifiers}' and key '{Key}'", mod, k);
				return;
			}

			e.SuppressEvent = true;

			_bus.Publish(new WtqHotkeyPressedEvent(mod, k));

			suppress1 = k;
			suppress2 = mod;
		};

		_hook.KeyReleased += (s, e) =>
		{
			e.SuppressEvent = false;

			var k = (Keys)e.Data.KeyCode;
			var m = GetModifiers(e.Data.KeyCode);

			mod ^= m;

			_log.LogDebug("KeyPressed(modifiers:{Modifiers} ({CumModifiers}), key:{Key})", m, k, mod);

			if ((suppress1 & k) == k)
			{
				Console.WriteLine("SUPPRESS K");
				suppress1 ^= k;
				//e.SuppressEvent = true;
			}

			if ((suppress2 & m) == m)
			{
				Console.WriteLine("SUPPRESS M");
				suppress2 ^= m;
				//e.SuppressEvent = true;
			}

		};

		_ = _hook.RunAsync();

		return Task.CompletedTask;
	}

	protected override ValueTask OnDisposeAsync()
	{
		_log.LogDebug("Disposing SharpHook");

		_hook?.Dispose();

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