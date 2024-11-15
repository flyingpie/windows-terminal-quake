namespace Wtq.Utils;

/// <summary>
/// Extension of the regular <see cref="SemaphoreSlim"/>, that returns <see cref="IDisposable"/> on <see cref="WaitAsync"/>.<br/>
/// This removes the need for the classic try/finally pattern, replacing it with a single line "using" instead.
/// </summary>
public sealed class WtqSemaphoreSlim : IDisposable
{
	private readonly SemaphoreSlim _lock;

	public WtqSemaphoreSlim()
		: this(1, 1)
	{
	}

	public WtqSemaphoreSlim(int initialCount, int maxCount)
	{
		_lock = new SemaphoreSlim(initialCount, maxCount);
	}

	public async Task<IDisposable> WaitAsync(CancellationToken cancellationToken)
	{
		await _lock.WaitAsync(cancellationToken).NoCtx();

		return new DisposableHandle(() => _lock.Release());
	}

	public Task<IDisposable> WaitOneSecondAsync()
	{
		return WaitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
	}

	public void Dispose()
	{
		_lock.Dispose();
	}
}