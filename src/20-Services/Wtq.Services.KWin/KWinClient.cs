// using Microsoft.Extensions.Hosting;
// using Wtq.Services.KWin.DBus;
// using Wtq.Services.KWin.Dto;
// using Wtq.Services.KWin.Utils;

// namespace Wtq.Services.KWin;

// /// <summary>
// /// TODO(MvdO): Here's most of the work we would need to refactor, since each of these calls results in a JS file write.<br/>
// /// Now, they are currently written to a shared memory mount, so they shouldn't touch an actual drive, but it's still not great.<br/>
// /// TODO: The "throw" statement doesn't work great, eg. not (always?) logged or anything.
// /// </summary>
// public class KWinClient : IKWinClient
// {
// 	private const string JsGetWindows = """
// 		let getWindows = () => {

// 			// KWin5
// 			if (typeof workspace.clientList === "function") {
// 				console.log("Fetching window list using 'workspace.clientList()' (KWin5)");
// 				return workspace.clientList();
// 			}
		
// 			// KWin6
// 			if (typeof workspace.windowList === "function") {
// 				console.log("Fetching window list using 'workspace.windowList()' (KWin6)");
// 				return workspace.windowList();
// 			}

// 			console.log("Could not find function to fetch windows, unsupported version of KWin perhaps?");
// 		};
// 		""";

// 	private readonly ILogger _log = Log.For<KWinClient>();

// 	private readonly KWinScriptExecutor _kwinScriptEx;
// 	private readonly KWinService _kwinDbus;

// 	private KWinSupportInformation? _suppInf;

// 	internal KWinClient(
// 		KWinScriptExecutor kwinScriptEx,
// 		KWinService kwinDbus)
// 	{
// 		_kwinScriptEx = Guard.Against.Null(kwinScriptEx);
// 		_kwinDbus = Guard.Against.Null(kwinDbus);
// 	}

// 	public async Task BringToForegroundAsync(
// 		KWinWindow window,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Bring to foreground window '{Window}'", window);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			console.log("Bring to foreground window with resource class '{{window.ResourceClass}}'");

// 			let isDone = false;

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}
			
// 				client.minimized = false;
// 				client.desktop = workspace.currentDesktop;
			
// 				// KWin5
// 				if (typeof workspace.activeClient === "object") {
// 					workspace.activeClient = client;
// 					isDone = true;
// 					break;
// 				}
			
// 				// KWin6
// 				if (typeof workspace.activeWindow === "object") {
// 					workspace.activeWindow = client;
// 					isDone = true;
// 					break;
// 				}

// 				throw "Could not find property on workspace for active window, unsupported version of KWin perhaps?";
// 			}

// 			if (!isDone) {
// 				console.log("[BringWindowToForeground] Did not find a window with resource class '{{window.ResourceClass}}'");
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}

// 	public async Task<ICollection<KWinWindow>> GetWindowListAsync(
// 		CancellationToken cancellationToken)
// 	{
// 		_log.LogTrace("Fetching list of windows");

// 		var id = Guid.NewGuid();

// 		var js =
// 			$$"""
// 			{{JsGetWindows}}

// 			let clients = getWindows()
// 				.map(client => {
// 					return {
// 						internalId: client.internalId,
// 						resourceClass: client.resourceClass,
// 						resourceName: client.resourceName
// 					};
// 				});

// 			callDBus("wtq.svc", "/wtq/kwin", "wtq.kwin", "SendResponse", "{{id}}", JSON.stringify(clients));
// 			""";

// 		var windows = await _kwinScriptEx.ExecuteAsync<ICollection<KWinWindow>>(id, js, cancellationToken).NoCtx();

// 		_log.LogTrace("Got {Count} windows", windows.Count);

// 		return windows;
// 	}

// 	public async Task<Point> GetCursorPosAsync(
// 		CancellationToken cancellationToken)
// 	{
// 		_log.LogTrace("Fetching cursor position");

// 		var id = Guid.NewGuid();

// 		var js =
// 			$$"""
// 			"use strict";

// 			callDBus(
// 				"wtq.svc",
// 				"/wtq/kwin",
// 				"wtq.kwin",
// 				"SendResponse",
// 				"{{id}}",
// 				JSON.stringify(workspace.cursorPos));
// 			""";

// 		var kwinPoint = await _kwinScriptEx.ExecuteAsync<KWinPoint>(id, js, cancellationToken).NoCtx();
// 		var point = kwinPoint.ToPoint();

// 		_log.LogTrace("Got cursor position {Position}", point);

// 		return point;
// 	}

// 	public async Task<KWinSupportInformation> GetSupportInformationAsync(
// 		CancellationToken cancellationToken)
// 	{
// 		_log.LogTrace("Fetching support information");

// 		// TODO: Exp							iring cache, doesn't handle cases well were screen configurtion is changed while wtq is running.
// 		if (_suppInf == null)
// 		{
// 			var str = await _kwinDbus.CreateKWin("/KWin").SupportInformationAsync().NoCtx();

// 			_suppInf = KWinSupportInformation.Parse(str);
// 		}

// 		return _suppInf;
// 	}

// 	public async Task MoveWindowAsync(
// 		KWinWindow window,
// 		Rectangle rect,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Moving window '{Window}' to '{Rectangle}'", window, rect);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			console.log("Moving window with resource class '{{window.ResourceClass}}'");

// 			let isDone = false;

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}

// 				console.log("Setting window '{{window.ResourceClass}}' to position ({{rect.X}}, {{rect.Y}}, {{rect.Width}}, {{rect.Height}})");

// 				// Note that it's important to set the entire "frameGeometry" object in one go, otherwise separate properties may become readonly,
// 				// allowing us to eg. only set the width, and not the height, or vice versa.
// 				// Not sure if this is a bug, but it took a bunch of time to figure out.
// 				client.frameGeometry = {
// 					x: {{rect.X}},
// 					y: {{rect.Y}},
// 					width: {{rect.Width}},
// 					height: {{rect.Height}}
// 				};

// 				isDone = true;
// 				break;
// 			}

// 			if (!isDone) {
// 				console.log("[Move] Did not find a window with resource class '{{window.ResourceClass}}'");
// 				throw "[Move] Did not find a window with resource class '{{window.ResourceClass}}'"; // 'throw' doesn't seem to do anything, maybe do logs with levels instead?
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}

// 	public async Task SetTaskbarIconVisibleAsync(
// 		KWinWindow window,
// 		bool isVisible,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Setting taskbar icon visibility for window '{Window}' to '{IsVisible}'", window, isVisible);

// 		var skip = JsUtils.ToJsBoolean(!isVisible);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			let isDone = false;

// 			console.log("Set taskbar icon visibility for window with resource class '{{window.ResourceClass}}' to '{{isVisible}}'");

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}

// 				client.skipPager = {{skip}};
// 				client.skipSwitcher = {{skip}};
// 				client.skipTaskbar = {{skip}};
// 				isDone = true;
// 				break;
// 			}

// 			if (!isDone) {
// 				console.log("[SetTaskbarIconVisible] Did not find a window with resource class '{{window.ResourceClass}}'");
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}

// 	public async Task SetWindowAlwaysOnTopAsync(
// 		KWinWindow window,
// 		bool isAlwaysOnTop,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Setting window 'always on top' state for window '{Window}' to '{IsAlwaysOnTop}'", window, isAlwaysOnTop);

// 		var keepAbove = JsUtils.ToJsBoolean(isAlwaysOnTop);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			console.log("Set window always on top for window with resource class '{{window.ResourceClass}}' to '{{isAlwaysOnTop}}'");

// 			let isDone = false;

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}
			
// 				client.keepAbove = {{keepAbove}};
// 				isDone = true;

// 				break;
// 			}

// 			if (!isDone) {
// 				console.log("[SetWindowAlwaysOnTop] Did not find a window with resource class '{{window.ResourceClass}}'");
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}

// 	public async Task SetWindowOpacityAsync(
// 		KWinWindow window,
// 		float opacity,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Setting window opacity for window '{Window}' to opacity '{Opacity}'", window, opacity);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			console.log("Setting opacity for window with resource class '{{window.ResourceClass}}' to '{{opacity}}'");

// 			let isDone = false;

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}
			
// 				client.opacity = {{opacity}};
// 				isDone = true;

// 				break;
// 			}

// 			if (!isDone) {
// 				console.log("[SetWindowOpacity] Did not find a window with resource class '{{window.ResourceClass}}'");
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}

// 	public async Task SetWindowVisibleAsync(
// 		KWinWindow window,
// 		bool isVisible,
// 		CancellationToken cancellationToken)
// 	{
// 		Guard.Against.Null(window);

// 		_log.LogTrace("Setting window visible state for window '{Window}' to '{IsVisible}'", window, isVisible);

// 		var minimized = JsUtils.ToJsBoolean(!isVisible);

// 		var js = $$"""
// 			"use strict";

// 			{{JsGetWindows}}

// 			console.log("Setting visibility for window with resource class '{{window.ResourceClass}}' to '{{isVisible}}'");

// 			let isDone = false;

// 			for (let client of getWindows())
// 			{
// 				if (client.resourceClass !== "{{window.ResourceClass}}") {
// 					continue;
// 				}
			
// 				client.minimized = {{minimized}};
// 				isDone = true;

// 				break;
// 			}

// 			if (!isDone) {
// 				console.log("[SetWindowVisible] Did not find a window with resource class '{{window.ResourceClass}}'");
// 			}
// 			""";

// 		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
// 	}
// }