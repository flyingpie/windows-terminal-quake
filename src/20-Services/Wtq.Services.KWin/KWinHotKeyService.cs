using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var gl = _kwinService.CreateKGlobalAccel("/kglobalaccel");
		var comp = _kwinService.CreateComponent("/component/kwin");
		var kwin = _kwinService.CreateKWin("/org/kde/KWin");

		var names = await comp.ShortcutNamesAsync();
		var inf = await comp.AllShortcutInfosAsync();
		var x = await gl.AllComponentsAsync();

//		var inf = await kwin.ActiveOutputNameAsync();
//		var p = await kwin.GetPropertiesAsync();

		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_001_scr", KeyModifiers.Control, Keys.D1);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_002_scr", KeyModifiers.Control, Keys.D2);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_003_scr", KeyModifiers.Control, Keys.D3);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_004_scr", KeyModifiers.Control, Keys.D4);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_005_scr", KeyModifiers.Control, Keys.Q);

		var dbg = 2;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{

	}
}