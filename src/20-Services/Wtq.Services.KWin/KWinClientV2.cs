using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Resources;

namespace Wtq.Services.KWin;

public class KWinClientV2 : IKWinClient, IHostedService
{
	private readonly IKWinScriptService _scriptService;
	private readonly Task<IWtqDBusObject> _wtqBusObj;

	public KWinClientV2(
		IKWinScriptService scriptService,
		Task<IWtqDBusObject> wtqBusObj)
	{
		_scriptService = scriptService;
		_wtqBusObj = wtqBusObj;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
	}

	private bool _isInitialized;

	private async Task InitializeAsync()
	{
		if (_isInitialized)
		{
			return;
		}

		// TODO: To somewhere else.
		var scriptId = "WTQ-v1";
		// var path = "/home/marco/wtq-script-1.js";
		var path = "/home/marco/ws/flyingpie/wtq_2/src/20-Services/Wtq.Services.KWin/Resources/WtqKWinScript.js";

		// var Js = _Resources.WtqKWinScript;
		// await File.WriteAllTextAsync(path, Js, CancellationToken.None).NoCtx();

		if (await _scriptService.IsScriptLoadedAsync(scriptId).NoCtx())
		{
			await _scriptService.UnloadScriptAsync(scriptId).NoCtx();
		}

		await _scriptService.LoadScriptAsync(path, scriptId).NoCtx();
		await _scriptService.StartAsync().NoCtx();

		// TODO: Keep around disposable
		_isInitialized = true;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
	{
		await InitializeAsync().NoCtx();

		var dbus = (WtqDBusObject)await _wtqBusObj;
		var resp = await dbus
			.SendCommandAsync(new()
			{
				Type = "GET_CURSOR_POS",
			})
			.NoCtx();

		return resp
			.GetParamsAs<KWinPoint>()
			.ToPoint();
	}

	// public async Task<KWinSupportInformation> GetSupportInformationAsync(
	// 	CancellationToken cancellationToken)
	// {
	// 	// _log.LogTrace("Fetching support information");
	//
	// 	// TODO: Expiring cache, doesn't handle cases well were screen configurtion is changed while wtq is running.
	// 	if (_suppInf == null)
	// 	{
	// 		var str = await _kwinDbus.CreateKWin("/KWin").SupportInformationAsync().NoCtx();
	//
	// 		_suppInf = KWinSupportInformation.Parse(str);
	// 	}
	//
	// 	return _suppInf;
	// }

	public class KWinGetWindowListResponse
	{
		[JsonPropertyName("windows")] public ICollection<KWinWindow> Windows { get; set; }
	};

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(CancellationToken cancellationToken)
	{
		await InitializeAsync().NoCtx();

		var dbus = (WtqDBusObject)await _wtqBusObj;
		var resp = await dbus
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

		var dbus = (WtqDBusObject)await _wtqBusObj;

		_ = await dbus
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
}