namespace Wtq.Utils;

public sealed class InitLock
{
	private readonly SemaphoreSlim _lock = new(1);
	private bool _isInit;

	public InitLock()
	{
	}

	public async Task InitAsync(Func<Task> action)
	{
		Guard.Against.Null(action);

		try
		{
			if (_isInit)
			{
				return;
			}

			await _lock.WaitAsync().NoCtx();

			if (_isInit)
			{
				return;
			}

			await action().NoCtx();

			_isInit = true;
		}
		finally
		{
			_lock.Release();
		}
	}
}