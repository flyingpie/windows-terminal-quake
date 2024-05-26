using Wtq.Data;

namespace Wtq.Utils;

public interface IWtqTween
{
	Task AnimateAsync(
		WtqRect from,
		WtqRect to,
		int durationMs,
		AnimationType animType,
		Func<WtqRect, Task> move);
}