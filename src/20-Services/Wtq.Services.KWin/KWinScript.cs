using Wtq.Services.KWin.DBus;

namespace Wtq.Services.KWin;

public class KWinScript : IAsyncDisposable
{
	private readonly Task<IWtqDBusObject> _wtqDBusObj;
	private readonly Scripting _scripting;

	public KWinScript(
		Task<IWtqDBusObject> wtqDBusObj,
		IKWinScriptService scriptService)
	{
		
	}

	public async ValueTask DisposeAsync()
	{
		
	}
}