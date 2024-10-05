namespace Wtq.Utils;

public class Retry : IRetry
{
	private static readonly ILogger _log = Log.For(typeof(Retry));

	public static IRetry Default { get; } = new Retry();

	public void Execute(Action action)
	{
		_ = Execute(() =>
		{
			action();

			return 1;
		});
	}

	[SuppressMessage("Usage", "VSTHRD002:Avoid problematic synchronous waits", Justification = "MvdO: Maybe move to Polly or something?")]
	public TResult Execute<TResult>(Func<TResult> action)
	{
		return
			ExecuteAsync(() => Task.FromResult(action()))
			.GetAwaiter()
			.GetResult();
	}

	public async Task ExecuteAsync(Func<Task> action)
	{
		Guard.Against.Null(action);

		_ = await
			ExecuteAsync(async () =>
			{
				await action().ConfigureAwait(false);

				return 0;
			})
			.ConfigureAwait(false);
	}

	public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
	{
		Guard.Against.Null(action);

		var maxAttempts = 5;
		var curAttempt = 0;

		while (true)
		{
			curAttempt++;

			try
			{
				return await action().ConfigureAwait(false);
			}
			catch (CancelRetryException ex)
			{
				_log.LogWarning(ex, "Cancelling retry");
				throw new WtqException("Retry cancelled");
			}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "[Attempt {CurrentAttempt}/{MaxAttempts}] Got exception {Message}", curAttempt, maxAttempts, ex.Message);

				if (curAttempt >= maxAttempts)
				{
					throw;
				}

				var wait = TimeSpan.FromMilliseconds(500);
				_log.LogInformation("Waiting '{Delay}' before next attempt", wait);
				await Task.Delay(wait).ConfigureAwait(false);
			}
		}
	}
}