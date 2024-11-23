namespace Wtq.Utils;

/// <summary>
/// Makes async initialization easier and thread safe.
/// </summary>
public sealed class InitLock : IDisposable
{
	private readonly SemaphoreSlim _lock = new(1);

	private bool _isInit;

	public async Task InitAsync(Func<Task> action)
	{
		Guard.Against.Null(action);

		// See if we're already initialized.
		if (_isInit)
		{
			return;
		}

		try
		{
			// Make sure we're not being initialized by another thread right now.
			await _lock.WaitAsync().NoCtx();

			// See if another thread did initialization while we were waiting.
			if (_isInit)
			{
				return;
			}

			// Initialize.
			await action().NoCtx();

			_isInit = true;
		}
		finally
		{
			_lock.Release();
		}
	}

	public void Dispose()
	{
		_lock.Dispose();
	}
}