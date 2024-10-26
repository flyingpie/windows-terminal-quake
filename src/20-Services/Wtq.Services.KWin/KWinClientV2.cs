using Wtq.Configuration;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Utils;

namespace Wtq.Services.KWin;

/// <summary>
/// Wraps functions in the wtq.kwin script.
/// </summary>
internal sealed class KWinClientV2 : IKWinClient
{
	private readonly ILogger _log = Log.For<KWinClientV2>();

	private readonly IDBusConnection _dbus;
	private readonly Initializer _init;
	private readonly IKWinScriptService _scriptService;
	private readonly WtqDBusObject _wtqBusObj;

	private KWinScript? _script;

	public KWinClientV2(
		IDBusConnection dbus,
		IKWinScriptService scriptService,
		IWtqDBusObject wtqBusObj)
	{
		_init = new Initializer<KWinClientV2>(InitializeAsync);

		_dbus = Guard.Against.Null(dbus);
		_scriptService = scriptService;
		_wtqBusObj = (WtqDBusObject)wtqBusObj; // TODO: Fix.
	}

	public async Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"BRING_WINDOW_TO_FOREGROUND",
				new
				{
					resourceClass = window.ResourceClass,
				})
			.NoCtx();
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		return (await _wtqBusObj
			.SendCommandAsync("GET_CURSOR_POS")
			.NoCtx())
			.GetParamsAs<KWinPoint>()
			.ToPoint();
	}

	public async Task<KWinWindow?> GetForegroundWindowAsync()
	{
		await _init.InitAsync().NoCtx();

		return (await _wtqBusObj
			.SendCommandAsync("GET_FOREGROUND_WINDOW")
			.NoCtx())
			.GetParamsAs<KWinWindow>();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken)
	{
		// _log.LogTrace("Fetching support information");

		var kwin = await _dbus.GetKWinAsync().NoCtx();

		var supportInfStr = await kwin.SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(supportInfStr);
	}

	public async Task<KWinWindow> GetWindowAsync(KWinWindow window)
	{
		await _init.InitAsync().NoCtx();

		var resp = await _wtqBusObj
			.SendCommandAsync(
				"GET_WINDOW",
				new
				{
					resourceClass = window.ResourceClass,
				})
			.NoCtx();

		return resp.GetParamsAs<KWinWindow>();
	}

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		var resp = await _wtqBusObj.SendCommandAsync("GET_WINDOW_LIST").NoCtx();

		return resp
			.GetParamsAs<KWinGetWindowListResponse>()
			.Windows;
	}

	public async Task MoveWindowAsync(
		KWinWindow window,
		Point location,
		CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"MOVE_WINDOW",
				new
				{
					resourceClass = window.ResourceClass,
					x = location.X,
					y = location.Y,
				})
			.NoCtx();

		var w = await GetWindowAsync(window).NoCtx();

		var actualLocation = w.FrameGeometry.ToPoint();

		if (actualLocation != location)
		{
			_log.LogWarning("Window '{Window}' did not have expected location '{ExpectedLocation}' after move (was '{ActualLocation}')", window, location, actualLocation);
		}
	}

	public async Task RegisterHotkeyAsync(string name, KeyModifiers mod, Keys key)
	{
		var kwinMod = "Ctrl";
		var kwinKey = key switch
		{
			Keys.D1 => "1",
			Keys.D2 => "2",
			Keys.D3 => "3",
			Keys.D4 => "4",
			Keys.D5 => "5",
			Keys.D6 => "6",
			Keys.Q => "q",
			_ => "1",
		};

		var kwinSequence = $"{kwinMod}+{kwinKey}";

		_ = await _wtqBusObj
			.SendCommandAsync(new("REGISTER_HOT_KEY")
			{
				Params = new
				{
					name = $"{name}_name",
					title = $"{name}_title",
					sequence = kwinSequence,
					mod = mod.ToString(),
					key = key.ToString(),
				},
			})
			.NoCtx();
	}

	public async Task ResizeWindowAsync(
		KWinWindow window,
		Size size,
		CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"RESIZE_WINDOW",
				new
				{
					resourceClass = window.ResourceClass,
					width = size.Width,
					height = size.Height,
				})
			.NoCtx();

		var w = await GetWindowAsync(window).NoCtx();
		var actualSize = w.FrameGeometry.ToSize();

		if (actualSize != size)
		{
			_log.LogWarning("Window '{Window}' did not have expected size '{ExpectedSize}' after resize (was '{ActualSize}')", window, size, actualSize);
		}
	}

	public async Task SetTaskbarIconVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();
		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_TASKBAR_ICON_VISIBLE",
				new
				{
					resourceClass = window.ResourceClass,
					isVisible = JsUtils.ToJsBoolean(isVisible),
				})
			.NoCtx();

		await GetWindowAsync(window).NoCtx();
	}

	public async Task SetWindowAlwaysOnTopAsync(KWinWindow window, bool isAlwaysOnTop, CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_ALWAYS_ON_TOP",
				new
				{
					resourceClass = window.ResourceClass,
					isAlwaysOnTop = JsUtils.ToJsBoolean(isAlwaysOnTop),
				})
			.NoCtx();

		await GetWindowAsync(window).NoCtx();
	}

	public async Task SetWindowOpacityAsync(KWinWindow window, float opacity, CancellationToken cancellationToken)
	{
		await _init.InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_OPACITY",
				new
				{
					resourceClass = window.ResourceClass,
					opacity = opacity,
				})
			.NoCtx();

		await GetWindowAsync(window).NoCtx();
	}

	public Task SetWindowVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		// await _init.InitAsync().NoCtx();

		// _ = await _wtqBusObj
		// 	.SendCommandAsync(
		// 		"SET_WINDOW_VISIBLE",
		// 		new
		// 		{
		// 			resourceClass = window.ResourceClass,
		// 			isVisible = JsUtils.ToJsBoolean(isVisible),
		// 		})
		// 	.NoCtx();

		// await GetWindowAsync(window).NoCtx();

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
		await _wtqBusObj.InitAsync().NoCtx(); // TODO: Remove, currently required to make sure DBus object is initialized.

		_script = await _scriptService.LoadScriptAsync("wtq.kwin.js").NoCtx();
	}
}