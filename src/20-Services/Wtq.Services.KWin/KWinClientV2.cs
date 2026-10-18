using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Scripting;

namespace Wtq.Services.KWin;

/// <summary>
/// Wraps functions in the wtq.kwin script.
/// </summary>
internal sealed class KWinClientV2(IDBusConnection dbus, IWtqDBusObject wtqBusObj) : IKWinClient
{
	private readonly ILogger _log = Log.For<KWinClientV2>();

	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);
	private readonly WtqDBusObject _wtqBusObj = (WtqDBusObject)wtqBusObj; // TODO: Fix.

	public async Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"BRING_WINDOW_TO_FOREGROUND",
				new { internalId = window.InternalId },
				cancellationToken
			)
			.NoCtx();
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
	{
		return (
			await _wtqBusObj.SendCommandAsync("GET_CURSOR_POS", null, cancellationToken).NoCtx()
		)
			.GetParamsAs<KWinPoint>()
			.ToPoint();
	}

	public async Task<KWinWindow?> GetForegroundWindowAsync(CancellationToken cancellationToken)
	{
		return (
			await _wtqBusObj
				.SendCommandAsync("GET_FOREGROUND_WINDOW", null, cancellationToken)
				.NoCtx()
		).GetParamsAs<KWinWindow>();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken
	)
	{
		var kwin = await _dbus.GetKWinAsync().NoCtx();

		var supportInfStr = await kwin.SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(supportInfStr);
	}

	public async Task<KWinWindow?> GetWindowAsync(
		KWinWindow window,
		CancellationToken cancellationToken
	)
	{
		var resp = await _wtqBusObj
			.SendCommandAsync(
				"GET_WINDOW",
				new { internalId = window.InternalId },
				cancellationToken
			)
			.NoCtx();

		return resp.GetParamsAs<KWinWindow>();
	}

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(
		CancellationToken cancellationToken
	)
	{
		var resp = await _wtqBusObj
			.SendCommandAsync("GET_WINDOW_LIST", null, cancellationToken)
			.NoCtx();

		return resp.GetParamsAs<KWinGetWindowListResponse>().Windows;
	}

	public async Task MoveWindowAsync(
		KWinWindow window,
		Point location,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"MOVE_WINDOW",
				new
				{
					internalId = window.InternalId,
					x = location.X,
					y = location.Y,
				},
				cancellationToken
			)
			.NoCtx();
	}

	public async Task PinWindowAsync(
		KWinWindow window,
		Rectangle rect,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"PIN_WINDOW",
				new
				{
					internalId = window.InternalId,
					x = rect.X,
					y = rect.Y,
					width = rect.Width,
					height = rect.Height,
				},
				cancellationToken
			)
			.NoCtx();
	}

	public async Task ResizeWindowAsync(
		KWinWindow window,
		Size size,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"RESIZE_WINDOW",
				new
				{
					internalId = window.InternalId,
					width = size.Width,
					height = size.Height,
				},
				cancellationToken
			)
			.NoCtx();
	}

	public async Task SetTaskbarIconVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_TASKBAR_ICON_VISIBLE",
				new { internalId = window.InternalId, isVisible = JsUtils.ToJsBoolean(isVisible) },
				cancellationToken
			)
			.NoCtx();
	}

	public async Task SetWindowAlwaysOnTopAsync(
		KWinWindow window,
		bool isAlwaysOnTop,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_ALWAYS_ON_TOP",
				new
				{
					internalId = window.InternalId,
					isAlwaysOnTop = JsUtils.ToJsBoolean(isAlwaysOnTop),
				},
				cancellationToken
			)
			.NoCtx();
	}

	public async Task SetWindowOpacityAsync(
		KWinWindow window,
		float opacity,
		CancellationToken cancellationToken
	)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_OPACITY",
				new { internalId = window.InternalId, opacity = opacity },
				cancellationToken
			)
			.NoCtx();
	}

	public async Task UnpinWindowAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		_ = await _wtqBusObj
			.SendCommandAsync(
				"UNPIN_WINDOW",
				new { internalId = window.InternalId },
				cancellationToken
			)
			.NoCtx();
	}
}
