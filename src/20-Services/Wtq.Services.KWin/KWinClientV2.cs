using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Input;
using Wtq.Services.KWin.Scripting;

namespace Wtq.Services.KWin;

/// <summary>
/// Wraps functions in the wtq.kwin script.
/// </summary>
internal sealed class KWinClientV2(
	IDBusConnection dbus,
	IKWinScriptService scriptService,
	IWtqDBusObject wtqBusObj)
	: IAsyncDisposable, IKWinClient
{
	/// <summary>
	/// Packaged KWin script, that's inside the WTQ binaries folder.
	/// </summary>
	private static readonly string _pathToWtqKwinJsSrc = WtqPaths.GetPathRelativeToWtqAppDir("wtq.kwin.js");

	/// <summary>
	/// Path to KWin script in the XDG cache folder, that is reachable by both sandboxed WTQ, and KWin.<br/>
	/// This is necessary when running WTQ as a Flatpak, where KWin can't see the files, since they're sandboxed.
	/// </summary>
	private static readonly string _pathToWtqKwinJsCache = Path.Combine(WtqPaths.GetWtqTempDir(), "wtq.kwin.js");

	private readonly ILogger _log = Log.For<KWinClientV2>();

	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);
	private readonly WtqDBusObject _wtqBusObj = (WtqDBusObject)wtqBusObj; // TODO: Fix.
	private readonly InitLock _lock = new();

	private KWinScript? _script;

	private async Task InitAsync()
	{
		await _lock
			.InitAsync(async () =>
			{
				// Setup WTQ DBus service for the KWin script to talk to.
				await _wtqBusObj.InitAsync().NoCtx();

				// Load KWin script.
				// Note that we're copying the KWin script to the cache dir, as that will be available to both the host and the sandbox, in the case we're running as a Flatpak.
				// For non-Flatpak, we could use the path to the KWin script directly, but that cannot be seen by KWin since KWin can't see in our sandbox.
				_log.LogDebug("Copying KWin script from '{Src}' to '{Dst}'", _pathToWtqKwinJsSrc, _pathToWtqKwinJsCache);

				File.Copy(_pathToWtqKwinJsSrc, _pathToWtqKwinJsCache, overwrite: true);

				_script = await scriptService.LoadScriptAsync(_pathToWtqKwinJsCache).NoCtx();
			})
			.NoCtx();
	}

	public async ValueTask DisposeAsync()
	{
		await (_script?.DisposeAsync() ?? ValueTask.CompletedTask).NoCtx();
	}

	public async Task BringToForegroundAsync(
		KWinWindow window,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"BRING_WINDOW_TO_FOREGROUND",
				new
				{
					internalId = window.InternalId,
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		return (await _wtqBusObj
				.SendCommandAsync("GET_CURSOR_POS", null, cancellationToken)
				.NoCtx())
			.GetParamsAs<KWinPoint>()
			.ToPoint();
	}

	public async Task<KWinWindow?> GetForegroundWindowAsync(
		CancellationToken cancellationToken)
	{
		return (await _wtqBusObj
				.SendCommandAsync("GET_FOREGROUND_WINDOW", null, cancellationToken)
				.NoCtx())
			.GetParamsAs<KWinWindow>();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		var kwin = await _dbus.GetKWinAsync().NoCtx();

		var supportInfStr = await kwin.SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(supportInfStr);
	}

	public async Task<KWinWindow?> GetWindowAsync(
		KWinWindow window,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		var resp = await _wtqBusObj
			.SendCommandAsync(
				"GET_WINDOW",
				new
				{
					internalId = window.InternalId,
				},
				cancellationToken)
			.NoCtx();

		return resp.GetParamsAs<KWinWindow>();
	}

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		var resp = await _wtqBusObj.SendCommandAsync("GET_WINDOW_LIST", null, cancellationToken).NoCtx();

		return resp
			.GetParamsAs<KWinGetWindowListResponse>()
			.Windows;
	}

	public async Task MoveWindowAsync(
		KWinWindow window,
		Point location,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"MOVE_WINDOW",
				new
				{
					internalId = window.InternalId,
					x = location.X,
					y = location.Y,
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task RegisterHotkeyAsync(
		string name,
		string description,
		KeySequence sequence,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				new("REGISTER_HOT_KEY")
				{
					Params = new
					{
						name = name,
						title = description,
						sequence = sequence.ToKWinString(),
						mod = sequence.Modifiers.ToString(),
						keyChar = sequence.KeyChar ?? string.Empty,
						keyCode = sequence.KeyCode.ToString(),
					},
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task ResizeWindowAsync(
		KWinWindow window,
		Size size,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"RESIZE_WINDOW",
				new
				{
					internalId = window.InternalId,
					width = size.Width,
					height = size.Height,
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task SetTaskbarIconVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_TASKBAR_ICON_VISIBLE",
				new
				{
					internalId = window.InternalId,
					isVisible = JsUtils.ToJsBoolean(isVisible),
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task SetWindowAlwaysOnTopAsync(
		KWinWindow window,
		bool isAlwaysOnTop,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_ALWAYS_ON_TOP",
				new
				{
					internalId = window.InternalId,
					isAlwaysOnTop = JsUtils.ToJsBoolean(isAlwaysOnTop),
				},
				cancellationToken)
			.NoCtx();
	}

	public async Task SetWindowOpacityAsync(
		KWinWindow window,
		float opacity,
		CancellationToken cancellationToken)
	{
		await InitAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_OPACITY",
				new
				{
					internalId = window.InternalId,
					opacity = opacity,
				},
				cancellationToken)
			.NoCtx();
	}
}