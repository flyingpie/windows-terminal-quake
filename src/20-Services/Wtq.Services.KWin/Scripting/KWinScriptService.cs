using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin.Scripting;

internal sealed class KWinScriptService(
	IDBusConnection dbus)
	: IKWinScriptService
{
	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);
	private readonly ILogger _log = Log.For<KWinScriptService>();

	public async Task<KWinScript> LoadScriptAsync(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		path = Path.GetFullPath(path);

		var id = Path.GetFileName(path);

		var scr = await _dbus.GetScriptingAsync().NoCtx();

		if (await scr.IsScriptLoadedAsync(id).NoCtx())
		{
			await scr.UnloadScriptAsync(id).NoCtx();
		}

		_log.LogDebug("Loading KWin script from path '{Path}'", path);

		if (!File.Exists(path))
		{
			throw new WtqException($"No such script file at path '{path}'.");
		}

		await scr.LoadScriptAsync(path, id).NoCtx();
		await scr.StartAsync().NoCtx();

		return new KWinScript(() => scr.UnloadScriptAsync(id));
	}
}