using System.Runtime.CompilerServices;

namespace Wtq.Utils;

public static class SystemExtensions
{
	[SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "MvdO: Whether or not to await is up to the caller.")]
	public static ConfiguredTaskAwaitable NoCtx(this Task task)
	{
		ArgumentNullException.ThrowIfNull(task);

		return task.ConfigureAwait(false);
	}

	[SuppressMessage("Usage", "VSTHRD003:Avoid awaiting foreign Tasks", Justification = "MvdO: Whether or not to await is up to the caller.")]
	public static ConfiguredTaskAwaitable<TResult> NoCtx<TResult>(this Task<TResult> task)
	{
		ArgumentNullException.ThrowIfNull(task);

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

	public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
	{

		using (var timeoutCancellationTokenSource = new CancellationTokenSource())
		{

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
			if (completedTask == task)
			{
				timeoutCancellationTokenSource.Cancel();
				return await task; // Very important in order to propagate exceptions
			}
			else
			{
				throw new TimeoutException("The operation has timed out.");
			}
		}
	}
}