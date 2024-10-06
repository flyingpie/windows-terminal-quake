using System.Text.Json.Serialization;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

internal sealed class KWinClientV2 : IKWinClient
{
	private readonly DBus.KWin _kwinDBusService;
	private readonly Initializer _init;
	private readonly IKWinScriptService _scriptService;
	private readonly WtqDBusObject _wtqBusObj;

	private KWinScript? _script;

	internal KWinClientV2(
		DBus.KWin kwinDBusService,
		IKWinScriptService scriptService,
		IWtqDBusObject wtqBusObj)
	{
		_init = new(InitializeAsync);

		_kwinDBusService = Guard.Against.Null(kwinDBusService);
		_scriptService = scriptService;
		_wtqBusObj = (WtqDBusObject)wtqBusObj; // TODO: Fix.
	}

	public Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
	{
		await InitializeAsync().NoCtx();

		var resp = await _wtqBusObj
			.SendCommandAsync(new()
			{
				Type = "GET_CURSOR_POS",
			})
			.NoCtx();

		return resp
			.GetParamsAs<KWinPoint>()
			.ToPoint();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken)
	{
		// _log.LogTrace("Fetching support information");

		var supportInfStr = await _kwinDBusService.SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(supportInfStr);
	}

	public class KWinGetWindowListResponse
	{
		[JsonPropertyName("windows")] public ICollection<KWinWindow> Windows { get; set; }
	};

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(CancellationToken cancellationToken)
	{
		await InitializeAsync().NoCtx();

		var resp = await _wtqBusObj
			.SendCommandAsync(new()
			{
				Type = "GET_WINDOW_LIST",
			})
			.NoCtx();

		return resp
			.GetParamsAs<KWinGetWindowListResponse>()
			.Windows;
	}

	public async Task MoveWindowAsync(KWinWindow window, Rectangle rect, CancellationToken cancellationToken)
	{
		await InitializeAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(new("MOVE_WINDOW")
			{
				Params = new
				{
					resourceClass = window.ResourceClass,
					x = rect.X,
					y = rect.Y,
					width = rect.Width,
					height = rect.Height,
				},
			})
			.NoCtx();
	}

	public Task SetTaskbarIconVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task SetWindowAlwaysOnTopAsync(KWinWindow window, bool isAlwaysOnTop, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task SetWindowOpacityAsync(KWinWindow window, float opacity, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task SetWindowVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async ValueTask DisposeAsync()
	{
		_init.Dispose();

		if (_script != null)
		{
			await _script.DisposeAsync().NoCtx();
		}
	}

	private async Task InitializeAsync()
	{
		// TODO: To somewhere else.
		// TODO: Build artifact?
		var scriptId = "WTQ-v1";
		// var path = "/home/marco/wtq-script-1.js";
		var path = "/home/marco/ws/flyingpie/wtq_2/src/20-Services/Wtq.Services.KWin/Resources/WtqKWinScript.js";

		_script = await _scriptService.LoadScriptAsync(path).NoCtx();

		// var Js = _Resources.WtqKWinScript;
		// await File.WriteAllTextAsync(path, Js, CancellationToken.None).NoCtx();

		// if (await _scriptService.IsScriptLoadedAsync(scriptId).NoCtx())
		// {
		// 	await _scriptService.UnloadScriptAsync(scriptId).NoCtx();
		// }

		// await _scriptService.LoadScriptAsync(path, scriptId).NoCtx();
		// await _scriptService.StartAsync().NoCtx();
	}
}