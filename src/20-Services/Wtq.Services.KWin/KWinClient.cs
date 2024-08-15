using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;
using Wtq.Services.KWin.Utils;
using Wtq.Utils;

namespace Wtq.Services.KWin;

public class KWinClient : IKWinClient
{
	private readonly KWinScriptExecutor _kwinScriptEx;
	private readonly KWinService _kwinDbus;

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

			for (let client of workspace.clientList())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.minimized = false;
				client.desktop = workspace.currentDesktop;
				workspace.activeClient = client;
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).NoCtx();
	}

	public async Task<IEnumerable<KWinWindow>> GetClientListAsync(
		CancellationToken cancellationToken)
	{
		var id = Guid.NewGuid();

		var js =
			$$"""
			let clients = workspace
				.clientList()
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
		var str = await _kwinDbus.CreateKWin("/KWin").SupportInformationAsync().NoCtx();

		return KWinSupportInformation.Parse(str);
	}

	public async Task MoveClientAsync(
		KWinWindow window,
		Rectangle rect,
		CancellationToken cancellationToken)
	{
		Guard.Against.Null(window);

		var js = $$"""
			"use strict";

			for (let client of workspace.clientList())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}

				client.frameGeometry = {
					x: {{rect.X}},
					y: {{rect.Y}},
					width: {{rect.Width}},
					height: {{rect.Height}}
				};
				
				break;
			}
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

			for (let client of workspace.clientList())
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

			for (let client of workspace.clientList())
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

			for (let client of workspace.clientList())
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

			for (let client of workspace.clientList())
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