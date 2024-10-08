using System.Text.Json.Serialization;
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
		await _init.InitializeAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"BRING_WINDOW_TO_FOREGROUND",
				new
				{
					resourceClass = window.ResourceClass,
				})
			.NoCtx();

		await GetWindowAsync(window).NoCtx();
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
	{
		await _init.InitializeAsync().NoCtx();

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

		var supportInfStr = await (await _dbus.GetKWinAsync().NoCtx()).SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(supportInfStr);
	}

	public class KWinGetWindowListResponse
	{
		[JsonPropertyName("windows")]
		public ICollection<KWinWindow> Windows { get; set; }
	}

	public async Task<ICollection<KWinWindow>> GetWindowListAsync(CancellationToken cancellationToken)
	{
		await _init.InitializeAsync().NoCtx();

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
		await _init.InitializeAsync().NoCtx();

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
		var fg = w.FrameGeometry.ToRect();

		if (fg != rect)
		{
			Console.WriteLine($"EXPECTED:{rect} ACTUAL:{fg}");
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

	public class KWinWindowInfo
	{
		[JsonPropertyName("resourceClass")]
		public string ResourceClass { get; set; }

		[JsonPropertyName("resourceName")]
		public string ResourceName { get; set; }

		[JsonPropertyName("frameGeometry")]
		public KWinRectangle FrameGeometry { get; set; }

		[JsonPropertyName("skipPager")]
		public bool SkipPager { get; set; }

		[JsonPropertyName("skipTaskbar")]
		public bool SkipTaskbar { get; set; }

		[JsonPropertyName("skipSwitcher")]
		public bool SkipSwitcher { get; set; }

		[JsonPropertyName("minimized")]
		public bool Minimized { get; set; }

		[JsonPropertyName("keepAbove")]
		public bool KeepAbove { get; set; }

		[JsonPropertyName("layer")]
		public int Layer { get; set; }

		[JsonPropertyName("hidden")]
		public bool Hidden { get; set; }

		public override string ToString() =>
			$"{ResourceClass} FrameGeometry:{FrameGeometry} SkipPager:{SkipPager} SkipTaskbar:{SkipTaskbar} SkipSwitcher:{SkipSwitcher} Minimized:{Minimized} KeepAbove:{KeepAbove} Layer:{Layer} Hidden:{Hidden}";
	}

	public async Task<KWinWindowInfo> GetWindowAsync(KWinWindow window)
	{
		await _init.InitializeAsync().NoCtx();

		var resp = await _wtqBusObj
			.SendCommandAsync(
				"GET_WINDOW",
				new
				{
					resourceClass = window.ResourceClass,
				})
			.NoCtx();

		Console.WriteLine($"WINDOW:${resp.GetParamsAs<KWinWindowInfo>()}");

		return resp.GetParamsAs<KWinWindowInfo>();
	}

	public async Task ResizeWindowAsync(KWinWindow window, Rectangle rect, CancellationToken cancellationToken)
	{
		await _init.InitializeAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"RESIZE_WINDOW",
				new
				{
					resourceClass = window.ResourceClass,
					width = rect.Width,
					height = rect.Height,
				})
			.NoCtx();

		var w = await GetWindowAsync(window).NoCtx();
		var fg = w.FrameGeometry.ToRect();

		if (fg != rect)
		{
			Console.WriteLine($"EXPECTED:{rect} ACTUAL:{fg}");
		}
	}

	public async Task SetTaskbarIconVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		await _init.InitializeAsync().NoCtx();

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
		await _init.InitializeAsync().NoCtx();

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
		await _init.InitializeAsync().NoCtx();

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

	public async Task SetWindowVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		await _init.InitializeAsync().NoCtx();

		_ = await _wtqBusObj
			.SendCommandAsync(
				"SET_WINDOW_VISIBLE",
				new
				{
					resourceClass = window.ResourceClass,
					isVisible = JsUtils.ToJsBoolean(isVisible),
				})
			.NoCtx();

		await GetWindowAsync(window).NoCtx();
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

		// TODO: Build artifact?
		var scriptId = "WTQ-v1";
		var path = "Resources/WtqKWinScript.js";

		_script = await _scriptService.LoadScriptAsync(scriptId, path).NoCtx();
	}
}