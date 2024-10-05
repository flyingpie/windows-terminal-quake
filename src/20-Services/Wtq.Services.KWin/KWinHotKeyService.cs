#pragma warning disable // PoC

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tmds.DBus.Protocol;
using Wtq.Configuration;
using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

internal class KWinHotKeyService : IDisposable, IHostedService
{
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly KWinScriptExecutor _scriptExecutor;
	private readonly KWinService _kwinService;

	public KWinHotKeyService(
		IOptionsMonitor<WtqOptions> opts,
		KWinScriptExecutor scriptExecutor,
		KWinService kwinService)
	{
		_opts = opts;
		_scriptExecutor = scriptExecutor;
		_kwinService = kwinService;
	}

	public void Dispose()
	{
		// Nothing to do.
	}

	private IDisposable _disp1;
	private IDisposable _disp2;

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var gl = _kwinService.CreateKGlobalAccel("/kglobalaccel");
		var comp = _kwinService.CreateComponent("/component/kwin");
		var kwin = _kwinService.CreateKWin("/org/kde/KWin");

		// Clear.
		for (int i = 0; i < 5; i++)
		{
			var resx1 = await gl.UnregisterAsync("kwin", $"wtq_hk1_00{i + 1}_scr_text");
		}

		await comp.CleanUpAsync();

		// TODO: Although we haven't gotten shortcut registration to work reliably through direct DBus calls,
		// we _can_ catch when shortcuts are being pressed/released.
		// So dial down the JS part to just registration, remove the callback to WTQ part.
		_disp1 = await comp.WatchGlobalShortcutPressedAsync((exception, tuple) =>
		{
			var xx2 = 2;
			Console.WriteLine($"PRESSED:{tuple.ComponentUnique} {tuple.ShortcutUnique} {tuple.Timestamp} {exception?.Message}");
		});

		_disp2 = await comp.WatchGlobalShortcutReleasedAsync((exception, tuple) =>
		{
			Console.WriteLine($"RELEASED:{tuple.ComponentUnique} {tuple.ShortcutUnique} {tuple.Timestamp} {exception?.Message}");
		});

		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_005_scr", KeyModifiers.Control, Keys.Q);

		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_001_scr", KeyModifiers.Control, Keys.D1);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_002_scr", KeyModifiers.Control, Keys.D2);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_003_scr", KeyModifiers.Control, Keys.D3);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_004_scr", KeyModifiers.Control, Keys.D4);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_006_scr", KeyModifiers.Control, Keys.D5);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_007_scr", KeyModifiers.Control, Keys.D6);
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// Nothing to do.
		return Task.CompletedTask;
	}
}