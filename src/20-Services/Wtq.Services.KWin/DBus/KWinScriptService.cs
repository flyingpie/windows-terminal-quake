using Wtq.Services.KWin;
using Wtq.Services.KWin.DBus;

public interface IKWinScriptService
{
	Task<KWinScript> LoadScriptAsync(string id, string path);
}

internal sealed class KWinScriptService(
	IDBusConnection dbus)
	: IKWinScriptService
{
	private readonly IDBusConnection _dbus = Guard.Against.Null(dbus);

	public async Task<KWinScript> LoadScriptAsync(string id, string path)
	{
		var scr = await _dbus.GetScriptingAsync().NoCtx();

		if (await scr.IsScriptLoadedAsync(id).NoCtx())
		{
			await scr.UnloadScriptAsync(id).NoCtx();
		}

		await scr.LoadScriptAsync(path, id).NoCtx();
		await scr.StartAsync().NoCtx();

		return new KWinScript(() => scr.UnloadScriptAsync(id));
	}
}