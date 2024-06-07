using System.Collections.Generic;
using Wtq.Data;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Dto;

namespace Wtq.Services.KWin;

public interface IKWinClient
{
	Task<IEnumerable<KWinWindow>> GetClientListAsync(CancellationToken cancellationToken);

	Task<Point> GetCursorPosAsync(CancellationToken cancellationToken);

	Task MoveClientAsync(KWinWindow window, WtqRect rect, CancellationToken cancellationToken);

	Task<KWinSupportInformation> GetSupportInformationAsync(CancellationToken cancellationToken);

	Task SetWindowOpacityAsync(KWinWindow window, float opacity, CancellationToken cancellationToken);

	Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken);

	Task SetTaskbarIconVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken);

	Task SetWindowVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken);

	Task SetWindowAlwaysOnTopAsync(KWinWindow window, bool isAlwaysOnTop, CancellationToken cancellationToken);
}

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

	public async Task<IEnumerable<KWinWindow>> GetClientListAsync(CancellationToken cancellationToken)
	{
		var id = Guid.NewGuid();

		var js =
			$$"""
			console.log("Before! [{{id}}]");
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

			console.log('After!');
			""";

		return await _kwinScriptEx.ExecuteAsync<IEnumerable<KWinWindow>>(id, js, cancellationToken).ConfigureAwait(false);
	}

	public async Task<Point> GetCursorPosAsync(CancellationToken cancellationToken)
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

		var point = await _kwinScriptEx.ExecuteAsync<KWinPoint>(id, js, cancellationToken).ConfigureAwait(false);

		return point.ToPoint();
	}

	public async Task<KWinSupportInformation> GetSupportInformationAsync(CancellationToken cancellationToken)
	{
		var str = await _kwinDbus.CreateKWin("/KWin").SupportInformationAsync().ConfigureAwait(false);

		return KWinSupportInformation.Parse(str);
	}

	public async Task MoveClientAsync(KWinWindow window, WtqRect rect, CancellationToken cancellationToken)
	{
		var js = $$"""
			"use strict";

			console.log("Setup");

			let internalId = "{4c02474c-b6c5-4052-9677-30ee924e66b5}";

			for (let client of workspace.clientList())
			{
				//if (client.resourceClass !== "org.wezfurlong.wezterm") {
				//	continue;
				//}
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				//client.minimized = false;
				//client.desktop = workspace.currentDesktop;
				//workspace.activeClient = client;
			
			
				console.log("GOT");
				client.frameGeometry = {
					x: {{rect.X}},
					y: {{rect.Y}},
					width: {{rect.Width}},
					height: {{rect.Height}}
				};
				
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}

	public async Task RegisterShortcutAsync()
	{
		var id = Guid.NewGuid();
		var js = $$"""
			registerShortcut(
				"WTQ_103",
				"WTQ_103",
				"Ctrl+q",
				() => {
					console.log("Fire shortcut ''");
					callDBus("wtq.svc", "/wtq/kwin", "wtq.kwin", "PressHotkey", "{{id}}", JSON.stringify(clients));
					console.log("SUP2!");
				}
			);
			""";
	}

	public async Task SetWindowOpacityAsync(KWinWindow window, float opacity, CancellationToken cancellationToken)
	{
		var js = $$"""
			"use strict";

			console.log("Setup");

			for (let client of workspace.clientList())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.opacity = {{opacity}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}

	public async Task BringToForegroundAsync(KWinWindow window, CancellationToken cancellationToken)
	{
		var js = $$"""
			"use strict";

			console.log("Setup");

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

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}

	public async Task SetTaskbarIconVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		var skip = (!isVisible).ToString().ToLowerInvariant();

		var js = $$"""
			"use strict";

			console.log("Setup");

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

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}

	public async Task SetWindowVisibleAsync(KWinWindow window, bool isVisible, CancellationToken cancellationToken)
	{
		var isVisibleStr = (!isVisible).ToString().ToLowerInvariant();

		var js = $$"""
			"use strict";

			console.log("Setup");

			for (let client of workspace.clientList())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.minimized = {{isVisibleStr}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}

	public async Task SetWindowAlwaysOnTopAsync(KWinWindow window, bool isAlwaysOnTop, CancellationToken cancellationToken)
	{
		var isAlwaysOnTopStr = isAlwaysOnTop.ToString().ToLowerInvariant();

		var js = $$"""
			"use strict";

			console.log("Setup");

			for (let client of workspace.clientList())
			{
				if (client.resourceClass !== "{{window.ResourceClass}}") {
					continue;
				}
			
				client.keepAbove = {{isAlwaysOnTopStr}};
			
				break;
			}
			""";

		await _kwinScriptEx.ExecuteAsync(js, cancellationToken).ConfigureAwait(false);
	}
}