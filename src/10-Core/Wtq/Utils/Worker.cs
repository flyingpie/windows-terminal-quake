namespace Wtq.Utils;

/// <summary>
/// Runs a specified task at a specified interval.
/// </summary>
public sealed class Worker : IAsyncDisposable
{
	public static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(250);

	private readonly ILogger _log = Log.For<Worker>();

	private readonly CancellationTokenSource _cts = new();
	private readonly TimeSpan _interval;

	public Worker(
		string name,
		Func<CancellationToken, Task> action)
		: this(name, action, DefaultInterval)
	{
	}

	public Worker(
		string name,
		Func<CancellationToken, Task>
		action,
		TimeSpan interval)
	{
		_interval = interval;

		_ = Task.Run(
			async () =>
			{
				while (!_cts.Token.IsCancellationRequested) // TODO: Token is disposed on app stop, which can throw an exception here.
				{
					try
					{
						await action(_cts.Token).NoCtx();
					}
					catch (Exception ex)
					{
						_log.LogWarning(ex, "Error running loop iteration: {Message}", ex.Message);
					}

					await Task.Delay(_interval).NoCtx();
				}
			});
	}

	public ValueTask DisposeAsync()
	{
		// TODO: Wait for loop iteration to end.
		_cts.Dispose();

		return ValueTask.CompletedTask;
	}
}