namespace Wtq.Utils;

public interface IRetry
{
	void Execute(Action action);

	TResult Execute<TResult>(Func<TResult> action);

	Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> action);
}