namespace Wtq.Utils;

public sealed class WtqSemaphoreSlim(
	int initialCount,
	int maxCount)
	: IDisposable
{
	private readonly SemaphoreSlim _lock = new(initialCount, maxCount);

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