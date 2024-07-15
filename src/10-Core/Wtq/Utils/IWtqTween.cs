using Wtq.Data;

namespace Wtq.Utils;

public interface IWtqTween
{
	Task AnimateAsync(
		WtqRect src,
		WtqRect dst,
		int durationMs,
		AnimationType animType,
		Action<WtqRect> move);
}