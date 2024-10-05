using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Utils;

namespace Wtq.Services.KWin;

/// <summary>
/// TODO(MvdO): Here's most of the work we would need to refactor, since each of these calls results in a JS file write.<br/>
/// Now, they are currently written to a shared memory mount, so they shouldn't touch an actual drive, but it's still not great.
/// </summary>
public class KWinClient : IKWinClient
{
	private const string JsGetWindows = """
		let getWindows = () => {

			// KWin5
			if (typeof workspace.clientList === "function") {
				return workspace.clientList();
			}
		
			// KWin6
			if (typeof workspace.windowList === "function") {
				return workspace.windowList();
			}
		
			throw "Could not find function to fetch windows, unsupported version of KWin perhaps?";
		};
		""";

	private readonly KWinScriptExecutor _kwinScriptEx;
	private readonly KWinService _kwinDbus;

	private KWinSupportInformation? _suppInf;

	internal KWinClient(
		KWinScriptExecutor kwinScriptEx,
		KWinService kwinDbus)
	{
		_kwinScriptEx = Guard.Against.Null(kwinScriptEx);
		_kwinDbus = Guard.Against.Null(kwinDbus);
	}

	public async Task BringToForegroundAsync(
		KWinWindow window,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var js = $$"""
			"use strict";

			let isDone = false;

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.minimized = false;
				client.desktop = workspace.currentDesktop;
			
				// KWin5
				if (typeof workspace.activeClient === "object") {
					workspace.activeClient = client;
					isDone = true;
					break;
				}
			
				// KWin6
				if (typeof workspace.activeWindow === "object") {
					workspace.activeWindow = client;
					isDone = true;
					break;
				}
			
				throw "Could not find property on workspace for active window, unsupported version of KWin perhaps?";
			}

			throw "[BringWindowToForeground] Did not find a window with resource class '{{window.ResourceClass}}'";
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task<IEnumerable<KWinWindow>> GetClientListAsync(
		CancellationToken cancellationToken)
	{
		var id = Guid.NewGuid();

		var js =
			$$"""
			{{JsGetWindows}}

			let clients = getWindows()
				.map(client => {
					return {
						internalId: client.internalId,
						resourceClass: client.resourceClass,
						resourceName: client.resourceName
					};
				});

			callDBus("wtq.svc", "/wtq/kwin", "wtq.kwin", "SendResponse", "{{id}}", JSON.stringify(clients));
			""";

		return await _kwinScriptEx.ExecuteAsync<IEnumerable<KWinWindow>>(id, js, cancellationToken).NoCtx();
	}

	public async Task<Point> GetCursorPosAsync(
		CancellationToken cancellationToken)
	{
		var id = Guid.NewGuid();

		var js =
			$$"""
			"use strict";

			callDBus(
				"wtq.svc",
				"/wtq/kwin",
				"wtq.kwin",
				"SendResponse",
				"{{id}}",
				JSON.stringify(workspace.cursorPos));
			""";

		var point = await _kwinScriptEx.ExecuteAsync<KWinPoint>(id, js, cancellationToken).NoCtx();

		return point.ToPoint();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(
		CancellationToken cancellationToken)
	{
		if (_suppInf == null)
		{
			var str = await _kwinDbus.CreateKWin("/KWin").SupportInformationAsync().NoCtx();

			_suppInf = KWinSupportInformation.Parse(str);
		}

		return _suppInf;
	}

	public async Task MoveClientAsync(
		KWinWindow window,
		Rectangle rect,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var js = $$"""
			"use strict";

			let isDone = false;

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}

				console.log("Setting client '{{window.ResourceClass}}' to position ({{rect.X}}, {{rect.Y}}, {{rect.Width}}, {{rect.Height}})");

				client.frameGeometry = {
					x: {{rect.X}},
					y: {{rect.Y}},
					width: {{rect.Width}},
					height: {{rect.Height}}
				};

				client.frameGeometry.x = {{rect.X}};
				client.frameGeometry.y = {{rect.Y}};
				client.frameGeometry.width = {{rect.Width}};
				client.frameGeometry.height = {{rect.Height}};

				isDone = true;
				break;
			}

			throw "[Move] Did not find a window with resource class '{{window.ResourceClass}}'";
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task SetTaskbarIconVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var skip = JsUtils.ToJsBoolean(!isVisible);

		var js = $$"""
			"use strict";

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.skipPager = {{skip}};
				client.skipSwitcher = {{skip}};
				client.skipTaskbar = {{skip}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task SetWindowAlwaysOnTopAsync(
		KWinWindow window,
		bool isAlwaysOnTop,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var keepAbove = JsUtils.ToJsBoolean(isAlwaysOnTop);

		var js = $$"""
			"use strict";

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.keepAbove = {{keepAbove}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task SetWindowOpacityAsync(
		KWinWindow window,
		float opacity,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var js = $$"""
			"use strict";

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.opacity = {{opacity}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task SetWindowVisibleAsync(
		KWinWindow window,
		bool isVisible,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var minimized = JsUtils.ToJsBoolean(!isVisible);

		var js = $$"""
			"use strict";

			{{JsGetWindows}}

			for (let client of getWindows())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.minimized = {{minimized}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}
}