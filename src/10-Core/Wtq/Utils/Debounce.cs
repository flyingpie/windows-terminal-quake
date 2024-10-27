using System.Collections.Concurrent;

namespace Wtq.Utils;

public sealed class Debounce : IDisposable
{
	private readonly ConcurrentQueue<int> _q = new();
	private readonly SemaphoreSlim _lock = new(1, 1);
	private readonly object _lock2 = new();
	private bool _isDisposed;

	public void Dispose()
	{
		lock (_lock2)
		{
			if (_isDisposed)
			{
				return;
			}

			_lock.Dispose();

			_isDisposed = true;
		}
	}

	public async Task<DebounceHandle> WaitAsync(CancellationToken cancellationToken)
	{
		try
		{
			_q.Enqueue(1);

			await _lock.WaitAsync(cancellationToken).NoCtx();
		}
		finally
		{
			_q.TryDequeue(out _);
		}

		return new DebounceHandle(() => _lock.Release())
		{
			CanContinue = _q.IsEmpty,
		};
	}

	public Task<DebounceHandle> WaitOneSecondAsync()
	{
		return WaitAsync(new CancellationTokenSource(TimeSpan.FromSeconds(1)).Token);
	}
}