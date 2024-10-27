namespace Wtq.Utils;

public sealed class DebounceHandle(Action onDispose) : IDisposable
{
	private readonly Action _onDispose = Guard.Against.Null(onDispose);

	public bool CanContinue { get; set; }

	public void Dispose()
	{
		_onDispose();
	}
}