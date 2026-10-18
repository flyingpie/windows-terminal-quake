using Microsoft.Extensions.Options;
using Tmds.DBus.Protocol;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.DBus.Generated;
using Wtq.Services.KWin.Input;

namespace Wtq.Services.KWin;

/// <summary>
/// Registers WTQ hotkeys directly with KGlobalAccel over D-Bus (no detour through the KWin script),
/// and listens for the <c>globalShortcutPressed</c> signal to detect activation.
/// </summary>
internal sealed class KWinHotkeyService : WtqHostedService
{
	/// <summary>
	/// All WTQ-generated shortcuts share this prefix, so we can find and clean them up
	/// between application restarts without keeping around state.
	/// </summary>
	private const string WtqShortcutPrefix = "wtq_hotkey";

	/// <summary>
	/// KGlobalAccel component (unique name) under which WTQ registers its hotkeys.
	/// </summary>
	private const string WtqComponentUnique = "wtq";

	/// <summary>
	/// KGlobalAccel component (friendly name) shown in e.g. the "Shortcuts" settings window.
	/// </summary>
	private const string WtqComponentFriendly = "WTQ";

	/// <summary>
	/// <c>KGlobalAccel::NoAutoloading</c>: use the given key sequence, instead of a previously saved one.
	/// </summary>
	private const uint NoAutoloading = 0x4;

	/// <summary>
	/// Component that WTQ hotkeys used to be registered under, back when they went through the KWin script.
	/// </summary>
	private const string LegacyComponentUnique = "kwin";

	private readonly ILogger _log = Log.For<KWinHotkeyService>();
	private readonly WtqSemaphoreSlim _lock = new();

	/// <summary>
	/// Maps registered KGlobalAccel shortcut ids to the key sequence that triggers them.
	/// </summary>
	private readonly ConcurrentDictionary<string, KeySequence> _registered = new(
		StringComparer.Ordinal
	);

	// Set in constructor.
	private readonly IDBusConnection _dbus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqBus _bus;

	// Set on service init.
	private KGlobalAccel _kGlobalAccel = null!;
	private Component _component = null!;
	private IDisposable? _shortcutPressedSubscription;

	public KWinHotkeyService(IDBusConnection dbus, IOptionsMonitor<WtqOptions> opts, IWtqBus bus)
	{
		_opts = Guard.Against.Null(opts);
		_dbus = Guard.Against.Null(dbus);
		_bus = Guard.Against.Null(bus);

		// Update registrations every time the settings file is reloaded.
		opts.OnChange(
			(opt, someString) =>
				_ = Task.Run(async () => await RegisterAllAsync(CancellationToken.None))
		);

		_bus.OnEvent<WtqSuspendHotkeysEvent>(async _ =>
			await UnregisterAllAsync(CancellationToken.None)
		);
		_bus.OnEvent<WtqResumeHotkeysEvent>(async _ =>
			await RegisterAllAsync(CancellationToken.None)
		);
	}

	protected override async Task OnStartAsync(CancellationToken cancellationToken)
	{
		var kglobalAccel = await _dbus.GetKGlobalAccelServiceAsync().NoCtx();
		_kGlobalAccel = kglobalAccel.CreateKGlobalAccel("/kglobalaccel");
		_component = kglobalAccel.CreateComponent($"/component/{WtqComponentUnique}");

		// Listen for shortcut activations.
		_shortcutPressedSubscription = await _component
			.WatchGlobalShortcutPressed(OnGlobalShortcutPressed)
			.NoCtx();

		// Older WTQ versions registered hotkeys through the KWin script, which put them under the "kwin" component.
		// Clean those up, so they don't linger as dead key bindings.
		await CleanUpLegacyShortcutsAsync(
				kglobalAccel.CreateComponent($"/component/{LegacyComponentUnique}")
			)
			.NoCtx();

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
		_shortcutPressedSubscription?.Dispose();
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
		await RegisterGlobalHotkeysAsync();

		// App-specific.
		await RegisterAppHotkeysAsync();
	}

	public async Task UnregisterAllAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Unregistering hotkeys");

		_registered.Clear();

		// The component object only exists while it has at least one registered shortcut.
		var ids = await GetRegisteredShortcutIdsAsync().NoCtx();
		if (ids.Count == 0)
		{
			return;
		}

		foreach (var id in ids)
		{
			if (await _kGlobalAccel.UnregisterAsync(WtqComponentUnique, id).NoCtx())
			{
				_log.LogDebug("Unregistered {Id}", id);
			}
			else
			{
				_log.LogWarning("Unregistering {Id} failed, not sure why :(", id);
			}
		}

		// Some GC-like flush.
		await _component.CleanUpAsync().NoCtx();
	}

	private async Task CleanUpLegacyShortcutsAsync(Component legacyComponent)
	{
		string[] names;

		try
		{
			names = await legacyComponent.ShortcutNamesAsync().NoCtx();
		}
		catch (DBusErrorReplyException)
		{
			// Legacy component doesn't exist; nothing to clean up.
			return;
		}

		foreach (
			var id in names.Where(n =>
				n.StartsWith(WtqShortcutPrefix, StringComparison.OrdinalIgnoreCase)
			)
		)
		{
			if (await _kGlobalAccel.UnregisterAsync(LegacyComponentUnique, id).NoCtx())
			{
				_log.LogInformation("Removed legacy hotkey '{Id}'", id);
			}
			else
			{
				_log.LogWarning("Removing legacy hotkey '{Id}' failed, not sure why :(", id);
			}
		}
	}

	private async Task RegisterGlobalHotkeysAsync()
	{
		var index = 0;

		foreach (var hk in _opts.CurrentValue.Hotkeys)
		{
			// Unique identifier.
			var id = $"{WtqShortcutPrefix}_global_{index++}";

			// Descriptive name that shows up in the "Shortcuts" window.
			var name = "WTQ hotkey - Global";

			await RegisterHotkeyAsync(id, name, hk.Sequence).NoCtx();
		}
	}

	private async Task RegisterAppHotkeysAsync()
	{
		foreach (var app in _opts.CurrentValue.Apps.OrderBy(a => a.Name))
		{
			var appName = app.Name.ToLowerInvariant();

			var index = 0;

			foreach (var hk in app.Hotkeys)
			{
				// Unique identifier.
				var id = $"{WtqShortcutPrefix}_app_{appName}_{index++}";

				// Descriptive name that shows up in the "Shortcuts" window.
				var name = $"WTQ hotkey - App - {app.Name}";

				await RegisterHotkeyAsync(id, name, hk.Sequence).NoCtx();
			}
		}
	}

	private async Task RegisterHotkeyAsync(string id, string title, KeySequence sequence)
	{
		if (sequence.IsEmpty)
		{
			_log.LogWarning("Skipping empty hotkey with id '{Id}'", id);
			return;
		}

		var actionId = new[] { WtqComponentUnique, id, WtqComponentFriendly, title };
		var key = sequence.ToQtKeyCombination();

		_log.LogInformation(
			"Registering hotkey '{Sequence}' with id '{Id}' (Qt key: {Key})",
			sequence,
			id,
			key
		);

		// Register the action, then force the configured key sequence onto it.
		await _kGlobalAccel.DoRegisterAsync(actionId).NoCtx();
		await _kGlobalAccel.SetShortcutKeysAsync(actionId, key, NoAutoloading).NoCtx();

		_registered[id] = sequence;
	}

	private async Task<ICollection<string>> GetRegisteredShortcutIdsAsync()
	{
		try
		{
			return (await _component.ShortcutNamesAsync().NoCtx())
				.Where(n => n.StartsWith(WtqShortcutPrefix, StringComparison.OrdinalIgnoreCase))
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
		}
		catch (DBusErrorReplyException)
		{
			// The component doesn't exist (yet), so there's nothing to clean up.
			return [];
		}
	}

	private void OnGlobalShortcutPressed(
		Exception? exception,
		(string ComponentUnique, string ShortcutUnique, long Timestamp) value
	)
	{
		if (exception != null)
		{
			_log.LogWarning(exception, "Error while watching for global shortcut presses");

			return;
		}

		// Ignore shortcuts that aren't ours (e.g. registered by other applications).
		if (!_registered.TryGetValue(value.ShortcutUnique, out var sequence))
		{
			return;
		}

		_log.LogInformation("Hotkey pressed: {Id}", value.ShortcutUnique);

		_bus.Publish(new WtqHotkeyPressedEvent(sequence));
	}
}
