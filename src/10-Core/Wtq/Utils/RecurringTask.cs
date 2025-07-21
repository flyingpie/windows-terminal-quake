namespace Wtq.Utils;

/// <summary>
/// Runs a specified task at a specified interval.
/// </summary>
public sealed class RecurringTask(
	string name,
	TimeSpan interval,
	Func<CancellationToken, Task> action)
	: IAsyncDisposable
{
	private readonly ILogger _log = Log.For<RecurringTask>();

	private readonly CancellationTokenSource _cts = new();
	private readonly TaskCompletionSource _tcs = new();

	private readonly Func<CancellationToken, Task> _action = Guard.Against.Null(action);

	private string Name { get; } =
		Guard.Against.NullOrWhiteSpace(name);

	public TimeSpan Interval { get; } =
		Guard.Against.OutOfRange(interval, nameof(interval), TimeSpan.FromMilliseconds(200), TimeSpan.FromMinutes(1));

	public bool IsRunning { get; private set; }

	public void Start()
	{
		if (IsRunning)
		{
			throw new InvalidOperationException($"Worker '{Name}' is already running.");
		}

		IsRunning = true;

		_ = Task.Run(async () =>
		{
			try
			{
				while (true)
				{
					if (_cts.IsCancellationRequested)
					{
						break;
					}

					try
					{
						await action(_cts.Token).NoCtx();
					}
					catch (Exception ex)
					{
						_log.LogWarning(ex, "Error running loop iteration: {Message}", ex.Message);
					}

					await Task.Delay(interval, _cts.Token).NoCtx();
				}
			}
			finally
			{
				_tcs.SetResult();
			}
		});
	}

	public async ValueTask DisposeAsync()
	{
		_log.LogInformation("Stopping worker '{Worker}'", Name);

		IsRunning = false;

		await _cts.CancelAsync().NoCtx();

		await _tcs.Task.NoCtx();

		_log.LogInformation("Stopped worker '{Worker}'", Name);
	}

	public override string ToString() => $"[{nameof(RecurringTask)}] {Name} ({Interval}) IsRunning:{IsRunning}";
}