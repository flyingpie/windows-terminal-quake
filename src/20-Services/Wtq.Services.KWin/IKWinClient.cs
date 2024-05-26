using System.Collections.Generic;
using Wtq.Data;
using Wtq.Services.KWin.DBus;
using Wtq.Services.KWin.Models;

namespace Wtq.Services.KWin;

public interface IKWinClient
{
	Task<IEnumerable<KWinWindow>> GetClientListAsync(CancellationToken cancellationToken);

	Task MoveClientAsync(KWinWindow window, WtqRect rect, CancellationToken cancellationToken);
}

public class KWinClient : IKWinClient
{
	private readonly KWinScriptExecutor _kwinScriptEx;

	internal KWinClient(KWinScriptExecutor kwinScriptEx)
	{
		_kwinScriptEx = Guard.Against.Null(kwinScriptEx);
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

				client.minimized = false;
				client.desktop = workspace.currentDesktop;
				workspace.activeClient = client;


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
}