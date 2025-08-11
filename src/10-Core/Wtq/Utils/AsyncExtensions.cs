using System.Runtime.CompilerServices;

namespace Wtq.Utils;

[SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "MvdO: Whether or not to await is up to the caller.")]
public static class AsyncExtensions
{
	public static ConfiguredTaskAwaitable NoCtx(this Task task)
	{
		_ = Guard.Against.Null(task);

		return task.ConfigureAwait(false);
	}

	public static ConfiguredTaskAwaitable<TResult> NoCtx<TResult>(this Task<TResult> task)
	{
		_ = Guard.Against.Null(task);

		return task.ConfigureAwait(false);
	}

	public static ConfiguredValueTaskAwaitable NoCtx(this ValueTask task)
	{
		return task.ConfigureAwait(false);
	}

	public static ConfiguredValueTaskAwaitable<TResult> NoCtx<TResult>(this ValueTask<TResult> task)
	{
		return task.ConfigureAwait(false);
	}

	public static async Task TimeoutAfterAsync(this Task task, TimeSpan timeout)
	{
		_ = Guard.Against.Null(task);

		using var timeoutCancellationTokenSource = new CancellationTokenSource();

		var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).NoCtx();

		if (completedTask != task)
		{
			throw new TimeoutException("The operation has timed out.");
		}

		await timeoutCancellationTokenSource.CancelAsync().NoCtx();

		await task.NoCtx(); // Very important in order to propagate exceptions.
	}

	public static async Task<TResult> TimeoutAfterAsync<TResult>(this Task<TResult> task, TimeSpan timeout)
	{
		_ = Guard.Against.Null(task);

		using var timeoutCancellationTokenSource = new CancellationTokenSource();

		var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).NoCtx();

		if (completedTask != task)
		{
			throw new TimeoutException("The operation has timed out.");
		}

		await timeoutCancellationTokenSource.CancelAsync().NoCtx();

		return await task.NoCtx(); // Very important in order to propagate exceptions.
	}

	public static async ValueTask TimeoutAfterAsync(this ValueTask task, TimeSpan timeout)
	{
		await task.AsTask().TimeoutAfterAsync(timeout);
	}
}