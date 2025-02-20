using Microsoft.Extensions.Options;
using System.Text.Json;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.DBus.Generated;

namespace Wtq.Services.KWin;

/// <summary>
/// TODO: DBus-only shortcut registration (we did get listening to key press events working, actual registration proved more difficult).
/// TODO: Fetch known WTQ shortcut names, instead of the fixed index-based names.
/// </summary>
internal sealed class KWinWtqHotkeyService : WtqHostedService
{
	private const string WtqShortcutPrefix = "wtq_hotkey";

	private readonly IKWinClient _kwinClient;
	private readonly IOptionsMonitor<WtqOptions> _opts;

	private readonly ILogger _log = Log.For<KWinWtqHotkeyService>();

	private readonly InitLock _init = new();
	private readonly WtqSemaphoreSlim _lock = new();

	private readonly IDBusConnection _dbus;

	public KWinWtqHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IKWinClient kwinClient,
		IDBusConnection dbus,
		IWtqBus bus)
	{
		_kwinClient = Guard.Against.Null(kwinClient);
		_opts = Guard.Against.Null(opts);
		_dbus = Guard.Against.Null(dbus);
		_ = Guard.Against.Null(bus);

		opts.OnChange((opt, someString) =>
		{
			_ = Task.Run(async () => await RegisterAllAsync(CancellationToken.None));
		});
	}

	private IDisposable? _disp;

	public async Task RegisterAllAsync(CancellationToken cancellationToken)
	{
		// Make this part thread-safe.
		_ = await _lock.WaitAsync(cancellationToken).NoCtx();

		// Setup services (move to constructor?).
		var kwin = await _dbus.GetKWinServiceAsync().NoCtx();
		var gl = kwin.CreateKGlobalAccel("/kglobalaccel");
		var comp = kwin.CreateComponent("/component/kwin");

		// Fetch existing WTQ shortcuts from KWin.
		var snames = await comp.ShortcutNamesAsync();
		var wtqSc = snames
			.Where(n => n.StartsWith(WtqShortcutPrefix, StringComparison.OrdinalIgnoreCase))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		_log.LogInformation("Existing KWin shortcuts: {Shortcuts}", string.Join(", ", wtqSc));

		var o = _opts.CurrentValue;

		// If "HotkeyReset" is "true", we don't have to worry about overwriting stuff in the KWin config panel.
		// Just remove everything every time, and start fresh.
		if (o.HotkeyReset)
		{
			await RemoveShortcutsAsync(wtqSc, gl, comp);
			wtqSc.Clear();
		}

		// Global.
		var globalHkIndex = 0;
		foreach (var hk in o.Hotkeys)
		{
			// Unique identifier.
			var id = $"{WtqShortcutPrefix}_global_{globalHkIndex++}";

			// Descriptive name that shows up in the "Shortcuts" window.
			var name = "WTQ hotkey - Global";

			_log.LogInformation("Registering global hotkey '{Key}' with id '{Id}'", hk, id);

			await _kwinClient.RegisterHotkeyAsync(id, name, hk.Modifiers, hk.Key, cancellationToken).NoCtx();

			wtqSc.Remove(id);
		}

		// App-specific.
		foreach (var app in o.Apps.OrderBy(a => a.Name))
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

				wtqSc.Remove(id);
			}
		}

		// Remove shortcuts that are no long present in the WTQ settings file.
		await RemoveShortcutsAsync(wtqSc, gl, comp).NoCtx();
	}


	private async Task RemoveShortcutsAsync(IEnumerable<string> wtqSc, KGlobalAccel gl, Component comp)
	{
		foreach (var shortcutName in wtqSc)
		{
			if (await gl.UnregisterAsync("kwin", shortcutName).NoCtx())
			{
				_log.LogInformation("Unregistered {Name}", shortcutName);
			}
		}

		// Some GC-like flush.
		await comp.CleanUpAsync().NoCtx();
	}

	private void Handler(Exception? arg1, (string ComponentUnique, string ShortcutUnique, long Timestamp) arg2)
	{
		_log.LogInformation($"SHORTCUT_HANDLE: Exc:{arg1?.Message} ComponentUnique:{arg2.ComponentUnique}, ShortcutUnique:{arg2.ShortcutUnique}, Timestamp:{arg2.Timestamp}");
	}

	protected override async Task OnStartAsync(CancellationToken cancellationToken)
	{
		await RegisterAllAsync(cancellationToken);

		await InitAsync().NoCtx();
	}

	protected override async Task OnStopAsync(CancellationToken cancellationToken)
	{
		await ResetShortcutsAsync().NoCtx();
	}

	protected override ValueTask OnDisposeAsync()
	{
		_init.Dispose();

		return ValueTask.CompletedTask;
	}

	// private static string GetShortcutName(int index) => $"wtq_hotkey_{index:000}";

	private async Task InitAsync()
	{
		await _init.InitAsync(ResetShortcutsAsync).NoCtx();
	}

	private async Task ResetShortcutsAsync()
	{
		// // return;
		// _log.LogDebug("Removing shortcuts");
		//
		// var kwin = await _dbus.GetKWinServiceAsync().NoCtx();
		//
		// var gl = kwin.CreateKGlobalAccel("/kglobalaccel");
		// var comp = kwin.CreateComponent("/component/kwin");
		//
		// // Remove individual shortcut registrations.
		// for (var i = 0; i < MaxShortcutCount; i++)
		// {
		// 	// var name = GetShortcutName(i);
		//
		// 	if (await gl.UnregisterAsync("kwin", name).NoCtx())
		// 	{
		// 		_log.LogDebug("Unregistered {Name}", name);
		// 	}
		// }
		//
		// // Some GC-like flush.
		// await comp.CleanUpAsync().NoCtx();
	}
}