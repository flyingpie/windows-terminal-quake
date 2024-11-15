namespace Wtq.Utils;

/// <summary>
/// Runs a specified task at a specified interval.
/// </summary>
public sealed class Worker : IAsyncDisposable
{
	public static readonly TimeSpan DefaultInterval = TimeSpan.FromMilliseconds(250);

	private readonly ILogger _log = Log.For<Worker>();

	private readonly Func<CancellationToken, Task> _action;
	private readonly TimeSpan _interval;

	private CancellationTokenSource _cts = new();

	public Worker(string name, Func<CancellationToken, Task> action)
		: this(name, action, DefaultInterval)
	{
	}

	public Worker(string name, Func<CancellationToken, Task> action, TimeSpan interval)
	{
		_action = action;
		_interval = interval;

		_ = Task.Run(async () =>
		{
			while (!_cts.IsCancellationRequested)
			{
				try
				{
					await action(_cts.Token).NoCtx();
				}
				catch (Exception ex)
				{
					_log.LogWarning(ex, "Error running iteration for loop: {Message}", ex.Message);
				}

				await Task.Delay(_interval).NoCtx();
			}
		});
	}

	public async ValueTask DisposeAsync()
	{
		await _cts.CancelAsync().NoCtx();
		// _cts.Dispose();
	}
}