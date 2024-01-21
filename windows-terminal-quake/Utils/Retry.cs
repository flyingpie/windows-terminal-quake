namespace Wtq.Utils;

public interface IRetry
{
	void Execute(Action action);

	TResult Execute<TResult>(Func<TResult> action);

	Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
}

public class Retry : IRetry
{
	private static readonly ILogger _log = Log.For(typeof(Retry));

	public void Execute(Action action)
	{
		_ = Execute(() =>
		{
			action();

			return 1;
		});
	}

	public TResult Execute<TResult>(Func<TResult> action)
	{
		return
			ExecuteAsync(() =>
			{
				return Task.FromResult(action());
			})
			.GetAwaiter()
			.GetResult();
	}

	public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action)
	{
		var maxAttempts = 10;
		var curAttempt = 0;

		while (true)
		{
			curAttempt++;

			try
			{
				return await action();
			}
			//catch (CancelRetryException ex)
			//{
			//	_log.LogWarning(ex, "Cancelling retry");
			//	throw;
			//}
			catch (Exception ex)
			{
				_log.LogWarning(ex, "[Attempt {CurrentAttempt}/{MaxAttempts}] Got exception {Message}", curAttempt, maxAttempts, ex.Message);

				if (curAttempt >= maxAttempts)
				{
					throw;
				}

				var wait = TimeSpan.FromSeconds(2);
				_log.LogInformation("Waiting '{Delay}' before next attempt", wait);
				await Task.Delay(wait);
			}
		}
	}
}