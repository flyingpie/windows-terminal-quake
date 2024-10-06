namespace Wtq.Utils;

public sealed class Initializer(
	Func<Task> init)
	: IDisposable
{
	private readonly Func<Task> _init = Guard.Against.Null(init);

	private readonly SemaphoreSlim _lock = new(1, 1);

	private bool _isInitialized;

	public void Dispose()
	{
		_lock.Dispose();
	}

	public async Task InitializeAsync()
	{
		// Check if we've already initialized.
		if (_isInitialized)
		{
			return;
		}

		// If not, make sure we're the only thread initializing.
		// TODO: Cancellation token.
		await _lock.WaitAsync().NoCtx();

		// Check again, since another thread could have initialized in the mean time.
		if (_isInitialized)
		{
			return;
		}

		// Finally, let's get to the initializing.
		try
		{
			await _init().NoCtx();
		}
		finally
		{
			_lock.Release();
		}

		_isInitialized = true;
	}
}