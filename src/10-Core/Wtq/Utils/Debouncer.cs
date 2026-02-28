namespace Wtq.Utils;

public class Debouncer
{
	private readonly int _delayMilliseconds;
	private CancellationTokenSource _cancellationTokenSource;

	public Debouncer(int delayMilliseconds = 300)
	{
		_delayMilliseconds = delayMilliseconds;
	}

	public void Debounce(Func<Task> action)
	{
		_cancellationTokenSource?.Cancel();
		_cancellationTokenSource = new CancellationTokenSource();

		var token = _cancellationTokenSource.Token;

		_ = Task.Run(
			async () =>
			{
				try
				{
					await Task.Delay(_delayMilliseconds, token).NoCtx();
					if (!token.IsCancellationRequested)
					{
						await action();
					}
				}
				catch (TaskCanceledException)
				{
					// Ignore — debounce was reset
				}
			},
			token);
	}
}