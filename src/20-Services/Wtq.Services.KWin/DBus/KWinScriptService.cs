using Wtq.Services.KWin;
using Wtq.Services.KWin.DBus;

public interface IKWinScriptService
{
	// Task<bool> IsScriptLoadedAsync(string scriptId);

	Task<KWinScript> LoadScriptAsync(string path);

	// Task LoadScriptAsync(string path, string scriptId);

	// Task StartAsync();

	// Task UnloadScriptAsync(string scriptId);

}

internal sealed class KWinScriptService(
	Scripting scripting)
	: IKWinScriptService
{
	private readonly Scripting _scripting = Guard.Against.Null(scripting);

	public async Task<KWinScript> LoadScriptAsync(string path)
	{
		var scriptId = Guid.NewGuid().ToString();

		// TODO: To somewhere else.
		// TODO: Build artifact?
		// var scriptId = "WTQ-v1";
		// var path = "/home/marco/wtq-script-1.js";
		// var path = "/home/marco/ws/flyingpie/wtq_2/src/20-Services/Wtq.Services.KWin/Resources/WtqKWinScript.js";

		// var Js = _Resources.WtqKWinScript;
		// await File.WriteAllTextAsync(path, Js, CancellationToken.None).NoCtx();

		if (await _scripting.IsScriptLoadedAsync(scriptId).NoCtx())
		{
			await _scripting.UnloadScriptAsync(scriptId).NoCtx();
		}

		await _scripting.LoadScriptAsync(path, scriptId).NoCtx();
		await _scripting.StartAsync().NoCtx();

		return new KWinScript(() => _scripting.UnloadScriptAsync(scriptId));
	}

	// public Task<bool> IsScriptLoadedAsync(string scriptId)
	// {
	// 	return _scripting.IsScriptLoadedAsync(scriptId);
	// }

	// public Task LoadScriptAsync(string path, string scriptId)
	// {
	// 	return _scripting.LoadScriptAsync(path, scriptId);
	// }

	// public Task StartAsync()
	// {
	// 	return _scripting.StartAsync();
	// }

	// public Task UnloadScriptAsync(string scriptId)
	// {
	// 	return _scripting.UnloadScriptAsync(scriptId);
	// }
}