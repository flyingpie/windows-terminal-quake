// namespace Wtq.Utils;
//
// public sealed class Initializer<TOwner>(Func<Task> init)
// 	: Initializer(init)
// {
// }
//
// /// <summary>
// /// Helper class to handle with initializing things, in an async and thread-safe way, and makes sure the init is only called once.
// /// </summary>
// /// <param name="init">Delegate to the thing that actually does the initialization.</param>
// public abstract class Initializer(
// 	Func<Task> init)
// 	: IDisposable
// {
// 	/// <summary>
// 	/// The thing to call for the actual initialization.
// 	/// </summary>
// 	private readonly Func<Task> _init = Guard.Against.Null(init);
//
// 	/// <summary>
// 	/// Used to make the init thread-safe.
// 	/// </summary>
// 	private readonly SemaphoreSlim _lock = new(1, 1);
//
// 	/// <summary>
// 	/// Whether the init has completed.
// 	/// </summary>
// 	private bool _isInitialized;
//
// 	public void Dispose()
// 	{
// 		_lock.Dispose();
// 	}
//
// 	public async Task InitAsync()
// 	{
// 		// Check if we've already initialized.
// 		if (_isInitialized)
// 		{
// 			return;
// 		}
//
// 		// If not, make sure we're the only thread initializing.
// 		// TODO: Cancellation token.
// 		await _lock.WaitAsync().NoCtx();
//
// 		// Check again, since another thread could have initialized while we were waiting for a lock.
// 		if (_isInitialized)
// 		{
// 			return;
// 		}
//
// 		// Finally, let's get to the initializing.
// 		try
// 		{
// 			await _init().NoCtx();
// 		}
// 		finally
// 		{
// 			_lock.Release();
// 		}
//
// 		_isInitialized = true;
// 	}
// }