namespace Wtq.Utils;

public interface IRetry
{
	void Execute(Action action);

	TResult Execute<TResult>(Func<TResult> action);

	Task ExecuteAsync(Func<Task> action);

	Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
}