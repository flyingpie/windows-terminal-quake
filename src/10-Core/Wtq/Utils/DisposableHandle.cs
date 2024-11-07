namespace Wtq.Utils;

/// <summary>
/// Handle object that accepts a delegate that will be called on dispose.<br/>
/// Useful for making types that can make use of the "using" expression, such as <see cref="WtqSemaphoreSlim"/>.
/// </summary>
public sealed class DisposableHandle(Action onDispose) : IDisposable
{
	private readonly object _lock = new();
	private readonly Action _onDispose = Guard.Against.Null(onDispose);

	private bool _isDisposed;

	public void Dispose()
	{
		lock (_lock)
		{
			if (_isDisposed)
			{
				return;
			}

			_onDispose();

			_isDisposed = true;
		}
	}
}