using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Wtq.Configuration;

namespace Wtq.Services.KWin;

internal class KWinHotKeyService : IDisposable, IHostedService
{
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly KWinScriptExecutor _scriptExecutor;

	public KWinHotKeyService(
		IOptionsMonitor<WtqOptions> opts,
		KWinScriptExecutor scriptExecutor)
	{
		_opts = opts;
		_scriptExecutor = scriptExecutor;
	}
	
	public void Dispose()
	{

	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_001", KeyModifiers.Control, Keys.D1);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_002", KeyModifiers.Control, Keys.D2);
		await _scriptExecutor.RegisterHotkeyAsync("wtq_hk1_003", KeyModifiers.Control, Keys.Q);

		var dbg = 2;
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{

	}
}