using Wtq.Data;

namespace Wtq.Services;

public interface IWtqTween
{
	Task AnimateAsync(
		WtqRect from,
		WtqRect to,
		int durationMs,
		AnimationType animType,
		Action<WtqRect> move);
}