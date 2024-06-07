using System.Runtime.CompilerServices;

namespace Wtq.Utils;

public static class SystemExtensions
{
	public static ConfiguredTaskAwaitable NoCtx(this Task task)
		=> task.ConfigureAwait(false);

	public static ConfiguredTaskAwaitable<TResult> NoCtx<TResult>(this Task<TResult> task)
		=> task.ConfigureAwait(false);
}