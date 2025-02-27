using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.DBus.Generated;

namespace Wtq.Services.KWin;

/// <summary>
/// TODO: DBus-only shortcut registration (we did get listening to key press events working, actual registration proved more difficult).
/// TODO: Fetch known WTQ shortcut names, instead of the fixed index-based names.
/// </summary>
internal sealed class KWinHotkeyService : WtqHostedService
{
	/// <summary>
	/// Hotkeys are registered as "shortcuts" in KWin, and they need at least an id, by which they're uniquely identified.<br/>
	/// We're using a prefix for all WTQ-generated shortcuts, so we can easily find them between application restarts and such,
	/// without keeping around state.
	/// </summary>
	private const string WtqShortcutPrefix = "wtq_hotkey";

	private readonly ILogger _log = Log.For<KWinHotkeyService>();
	private readonly WtqSemaphoreSlim _lock = new();

	// Set in constructor.
	private readonly IDBusConnection _dbus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IKWinClient _kwinClient;
	private readonly WtqDBusObject _dbusObj;
	private readonly IWtqBus _bus;

	// Set on service init.
	private KWinService _kwinService = null!;
	private KGlobalAccel _kGlobalAccel = null!;
	private Component _kwinComponent = null!;

	public KWinHotkeyService(
		IDBusConnection dbus,
		IWtqDBusObject dbusObj,
		IOptionsMonitor<WtqOptions> opts,
		IKWinClient kwinClient,
		IWtqBus bus)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
		_opts = Guard.Against.Null(opts);
		_dbus = Guard.Against.Null(dbus);
		_dbusObj = Guard.Against.Null(dbusObj as WtqDBusObject); // TODO: Make nicer.
		_bus = Guard.Against.Null(bus);

		// Update registrations every time the settings file is reloaded.
		opts.OnChange((opt, someString) => _ = Task.Run(async () => await RegisterAllAsync(CancellationToken.None)));

		_bus.OnEvent<WtqSuspendHotkeysEvent>(async _ => await UnregisterAllAsync(CancellationToken.None));
		_bus.OnEvent<WtqResumeHotkeysEvent>(async _ => await RegisterAllAsync(CancellationToken.None));
	}

	protected override async Task OnStartAsync(CancellationToken cancellationToken)
	{
		// Setup services (move to constructor?).
		_kwinService = await _dbus.GetKWinServiceAsync().NoCtx();
		_kGlobalAccel = _kwinService.CreateKGlobalAccel("/kglobalaccel");
		_kwinComponent = _kwinService.CreateComponent("/component/kwin");

		_dbusObj.OnPressShortcut((arg) =>
		{
			_bus.Publish(new WtqHotkeyPressedEvent(arg.Mod, arg.Key));

			return Task.CompletedTask;
		});

		// Register hotkeys on WTQ start.
		await RegisterAllAsync(cancellationToken);
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		// Cleanup shortcuts (unless hotkey reset has been disabled).
		await UnregisterAllAsync(cancellationToken).NoCtx();
	}

	protected override ValueTask OnDisposeAsync()
	{
		_lock.Dispose();

		return ValueTask.CompletedTask;
	}

	public async Task RegisterAllAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Registering hotkeys");

		// Make this part thread-safe.
		using var l = await _lock.WaitAsync(cancellationToken).NoCtx();

		// Reset all shortcuts.
		await UnregisterAllAsync(cancellationToken);

		// Global.
		await RegisterGlobalHotkeysAsync(cancellationToken);

		// App-specific.
		await RegisterAppHotkeysAsync(cancellationToken);
	}

	public async Task UnregisterAllAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Unregistering hotkeys");

		// Fetch existing WTQ shortcuts from KWin.
		var kwinShortcutNames = (await _kwinComponent.ShortcutNamesAsync())
			.Where(n => n.StartsWith(WtqShortcutPrefix, StringComparison.OrdinalIgnoreCase))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		_log.LogInformation("Existing KWin shortcuts: {Shortcuts}", string.Join(", ", kwinShortcutNames));

		foreach (var kwinShortcutName in kwinShortcutNames)
		{
			if (await _kGlobalAccel.UnregisterAsync("kwin", kwinShortcutName).NoCtx())
			{
				_log.LogInformation("Unregistered {Name}", kwinShortcutName);
			}
			else
			{
				_log.LogWarning("Unregistered {Name} failed, not sure why :(", kwinShortcutName);
			}
		}

		// Some GC-like flush.
		await _kwinComponent.CleanUpAsync().NoCtx();
	}

	private async Task RegisterGlobalHotkeysAsync(CancellationToken cancellationToken)
	{
		var globalHkIndex = 0;
		foreach (var hk in _opts.CurrentValue.Hotkeys)
		{
			// Unique identifier.
			var id = $"{WtqShortcutPrefix}_global_{globalHkIndex++}";

			// Descriptive name that shows up in the "Shortcuts" window.
			var name = "WTQ hotkey - Global";

			_log.LogInformation("Registering global hotkey '{Key}' with id '{Id}'", hk, id);

			await _kwinClient.RegisterHotkeyAsync(id, name, hk.Modifiers, hk.Key, cancellationToken).NoCtx();
		}
	}

	private async Task RegisterAppHotkeysAsync(CancellationToken cancellationToken)
	{
		foreach (var app in _opts.CurrentValue.Apps.OrderBy(a => a.Name))
		{
			var appName = app.Name.ToLowerInvariant();

			var appHkIndex = 0;
			foreach (var hk in app.Hotkeys)
			{
				// Unique identifier.
				var id = $"{WtqShortcutPrefix}_app_{appName}_{appHkIndex++}";

				// Descriptive name that shows up in the "Shortcuts" window.
				var name = $"WTQ hotkey - App - {app.Name}";

				_log.LogInformation("Registering app hotkey '{Key}' with id '{Id}', for app '{App}'", hk, id, app);

				await _kwinClient.RegisterHotkeyAsync(id, name, hk.Modifiers, hk.Key, cancellationToken).NoCtx();
			}
		}
	}
}