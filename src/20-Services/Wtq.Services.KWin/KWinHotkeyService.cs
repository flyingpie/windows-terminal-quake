using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

/// <summary>
/// TODO: DBus-only shortcut registration (we did get listening to key press events working, actual registration proved more difficult).
/// TODO: Fetch known WTQ shortcut names, instead of the fixed index-based names.
/// </summary>
internal sealed class KWinHotkeyService
	: IDisposable, IHostedService
{
	/// <summary>
	/// To de-register any left-over shortcuts on WTQ start, we need to know their names.<br/>
	/// However, we don't have a nice way of finding out what those names are.<br/>
	///
	/// So instead, we use an index-based naming scheme, so we get deterministic names.
	/// </summary>
	private const int MaxShortcutCount = 50;

	private readonly ILogger _log = Log.For<KWinHotkeyService>();
	private readonly InitLock _init = new();

	private readonly IDBusConnection _dbus;

	private int _shortcutIndex;

	public KWinHotkeyService(
		IKWinClient kwinClient,
		IDBusConnection dbus,
		IWtqBus bus)
	{
		_dbus = Guard.Against.Null(dbus);
		_ = Guard.Against.Null(bus);

		bus.OnEvent<WtqHotkeyDefinedEvent>(
			async e =>
			{
				await InitAsync().NoCtx();

				var name = GetShortcutName(_shortcutIndex++);

				_log.LogInformation("Registering shortcut with name '{Name}', modifiers '{Modifiers}' and key '{Key}'", name, e.Modifiers, e.Key);

				await kwinClient.RegisterHotkeyAsync(name, e.AppOptions?.Name ?? string.Empty, e.Modifiers, e.Key, CancellationToken.None).NoCtx();
			});
	}

	public void Dispose()
	{
		_init.Dispose();
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		await ResetShortcutsAsync().NoCtx();
	}

	private static string GetShortcutName(int index) => $"wtq_hotkey_{index:000}";

	private async Task InitAsync()
	{
		await _init.InitAsync(ResetShortcutsAsync).NoCtx();
	}

	private async Task ResetShortcutsAsync()
	{
		_log.LogDebug("Removing shortcuts");

		var kwin = await _dbus.GetKWinServiceAsync().NoCtx();

		var gl = kwin.CreateKGlobalAccel("/kglobalaccel");
		var comp = kwin.CreateComponent("/component/kwin");

		// Remove individual shortcut registrations.
		for (var i = 0; i < MaxShortcutCount; i++)
		{
			var name = GetShortcutName(i);

			if (await gl.UnregisterAsync("kwin", name).NoCtx())
			{
				_log.LogDebug("Unregistered {Name}", name);
			}
		}

		// Some GC-like flush.
		await comp.CleanUpAsync().NoCtx();
	}
}