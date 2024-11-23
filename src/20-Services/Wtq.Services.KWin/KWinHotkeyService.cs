#pragma warning disable // PoC

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wtq.Configuration;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

internal class KWinHotkeyService : IHostedService
{
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IKWinClient _kwinClient;
	private readonly IDBusConnection _dbus;

	public KWinHotkeyService(
		IOptionsMonitor<WtqOptions> opts,
		IKWinClient kwinClient,
		IDBusConnection dbus)
	{
		_opts = opts;
		_kwinClient = kwinClient;
		_dbus = dbus;
	}

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
		}

		await comp.CleanUpAsync();

		// TODO: Although we haven't gotten shortcut registration to work reliably through direct DBus calls,
		// we _can_ catch when shortcuts are being pressed/released.
		// So dial down the JS part to just registration, remove the callback to WTQ part.

		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_000_scr", KeyModifiers.Control, Keys.Q, cancellationToken);

		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_001_scr", KeyModifiers.Control, Keys.D1, cancellationToken);
		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_002_scr", KeyModifiers.Control, Keys.D2, cancellationToken);
		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_003_scr", KeyModifiers.Control, Keys.D3, cancellationToken);
		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_004_scr", KeyModifiers.Control, Keys.D4, cancellationToken);
		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_005_scr", KeyModifiers.Control, Keys.D5, cancellationToken);
		await _kwinClient.RegisterHotkeyAsync("wtq_hk1_006_scr", KeyModifiers.Control, Keys.D6, cancellationToken);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// TODO: Cleanup.
		return Task.CompletedTask;
	}
}