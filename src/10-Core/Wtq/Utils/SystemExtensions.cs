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
}