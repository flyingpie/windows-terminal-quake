namespace Wtq.Services.KWin.Scripting;

public sealed class KWinScript(Func<Task> onDispose) : System.IAsyncDisposable
{
	private readonly Func<Task> _onDispose = Guard.Against.Null(onDispose);

	public async ValueTask DisposeAsync()
	{
		await _onDispose().NoCtx();
	}
}
