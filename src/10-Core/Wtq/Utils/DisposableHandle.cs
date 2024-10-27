namespace Wtq.Utils;

public sealed class DisposableHandle(Action onDispose) : IDisposable
{
	private readonly Action _onDispose = Guard.Against.Null(onDispose);

	public void Dispose()
	{
		_onDispose();
	}
}