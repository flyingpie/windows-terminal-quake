namespace Wtq.Utils;

/// <summary>
/// Utils for running animation sequences.
/// </summary>
public interface IWtqTween
{
	/// <summary>
	/// Starts a sequence where a rectangle is moved from <paramref name="src"/> to <paramref name="dst"/>.<br/>
	/// The sequence will take ~<paramref name="durationMs"/> and progresses along a curve as specified by <paramref name="animType"/>.<br/>
	/// The <paramref name="move"/> callback is called with a progressing rectangle (position + size).<br/>
	/// </summary>
	Task AnimateAsync(
		Rectangle src,
		Rectangle dst,
		int durationMs,
		AnimationType animType,
		Func<Rectangle, Task> move);
}