namespace Wtq.Utils;

/// <summary>
/// Runs a specified task at a specified interval.
/// </summary>
public sealed class Worker : IAsyncDisposable
{
	private readonly ILogger _log = Log.For<Worker>();

	private readonly CancellationTokenSource _cts = new();
	private readonly TaskCompletionSource _tcs = new();

	private readonly string _name;

	public Worker(string name, TimeSpan interval, Func<CancellationToken, Task> action)
	{
		_name = Guard.Against.NullOrWhiteSpace(name);
		Guard.Against.Null(action);

		_ = Task.Run(async () =>
		{
			while (true)
			{
				try
				{
					await action(_cts.Token).NoCtx();
				}
				catch (Exception ex)
				{
					_log.LogWarning(ex, "Error running loop iteration: {Message}", ex.Message);
				}

				if (_cts.IsCancellationRequested)
				{
					_tcs.SetResult();
					break;
				}

				await Task.Delay(interval).NoCtx();
			}
		});
	}

	public async ValueTask DisposeAsync()
	{
		_log.LogInformation("Stopping worker '{Worker}'", _name);

		await _cts.CancelAsync().NoCtx();

		await _tcs.Task.NoCtx();

		_cts.Dispose();

		_log.LogInformation("Stopped worker '{Worker}'", _name);
	}
}