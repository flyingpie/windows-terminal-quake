#pragma warning disable // PoC

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

internal class KWinHotkeyService : IHostedService
{
	private readonly IDBusConnection _dbus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IKWinClient _kwinClient;
	private readonly IWtqBus _bus;

	public KWinHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IKWinClient kwinClient,
		IDBusConnection dbus,
		IWtqBus bus)
	{
		_opts = opts;
		_kwinClient = kwinClient;
		_dbus = dbus;
		_bus = bus;
	}

	private IDisposable _disp1;
	private IDisposable _disp2;

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var kwinx = await _dbus.GetKWinServiceAsync();

		var gl = kwinx.CreateKGlobalAccel("/kglobalaccel");
		var comp = kwinx.CreateComponent("/component/kwin");
		// var kwin = kwinx.CreateKWin("/org/kde/KWin");

		// Clear.
		for (var i = 0; i < 50; i++)
		{
			var resx1 = await gl.UnregisterAsync("kwin", $"wtq_hk1_{i:000}_scr_text");

			var resx2 = await gl.UnregisterAsync("kwin", $"wtq_hk1_{i:000}_scr_name");

			var resx3 = await gl.UnregisterAsync("kwin", $"wtq_hk1_{i:000}_scr");

			var resx4 = await gl.UnregisterAsync("kwin", $"wtq_hk_{i:000}_scr");

			var dbg = 2;
		}

		await comp.CleanUpAsync();

		var keys = new Dictionary<string, (KeyModifiers, Keys)>()
		{
			{ "wtq_hk_000", (KeyModifiers.Control, Keys.Q)  },
			{ "wtq_hk_001", (KeyModifiers.Control, Keys.D1) },
			{ "wtq_hk_002", (KeyModifiers.Control, Keys.D2) },
			{ "wtq_hk_003", (KeyModifiers.Control, Keys.D3) },
			{ "wtq_hk_004", (KeyModifiers.Control, Keys.D4) },
			{ "wtq_hk_005", (KeyModifiers.Control, Keys.D5) },
			{ "wtq_hk_006", (KeyModifiers.Control, Keys.D6) },
		};

		// TODO: Although we haven't gotten shortcut registration to work reliably through direct DBus calls,
		// we _can_ catch when shortcuts are being pressed/released.
		// So dial down the JS part to just registration, remove the callback to WTQ part.
		_disp1 = await comp.WatchGlobalShortcutPressedAsync((exception, tuple) =>
		{
			if (!keys.TryGetValue(tuple.ShortcutUnique, out var k))
			{
				return;
			}

			Console.WriteLine($"[{DateTimeOffset.UtcNow:s}] PRESSED:{k.Item1} + {k.Item2}");
			// Console.WriteLine($"PRESSED:{tuple.ComponentUnique} {tuple.ShortcutUnique} {tuple.Timestamp} {exception?.Message}");

			_bus.Publish(
				new WtqHotkeyPressedEvent()
				{
					Key = k.Item2,
					Modifiers = k.Item1,
				});
		});

		_disp2 = await comp.WatchGlobalShortcutReleasedAsync((exception, tuple) =>
		{
			if (!keys.TryGetValue(tuple.ShortcutUnique, out var k))
			{
				return;
			}

			Console.WriteLine($"[{DateTimeOffset.UtcNow:s}] RELEASED:{k.Item1} + {k.Item2}");

			// Console.WriteLine($"RELEASED:{tuple.ComponentUnique} {tuple.ShortcutUnique} {tuple.Timestamp} {exception?.Message}");

			_bus.Publish(
				new WtqHotkeyReleasedEvent()
				{
					Key = k.Item2,
					Modifiers = k.Item1,
				});
		});

		foreach (var k in keys)
		{
			await _kwinClient.RegisterHotkeyAsync(k.Key, k.Value.Item1, k.Value.Item2, cancellationToken);
		}

		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_000_scr", KeyModifiers.Control, Keys.Q, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_001_scr", KeyModifiers.Control, Keys.D1, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_002_scr", KeyModifiers.Control, Keys.D2, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_003_scr", KeyModifiers.Control, Keys.D3, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_004_scr", KeyModifiers.Control, Keys.D4, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_005_scr", KeyModifiers.Control, Keys.D5, cancellationToken);
		// await _kwinClient.RegisterHotkeyAsync("wtq_hk1_006_scr", KeyModifiers.Control, Keys.D6, cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}