using Wtq.Data;

namespace Wtq.Services;

public sealed class WtqTween : IWtqTween
{
	private const float FrameTimeMs = 1000f / 40f; // 40 = FPS

	private readonly ILogger _log = Log.For<WtqTween>();

	public async Task AnimateAsync(
		WtqRect from,
		WtqRect to,
		int durationMs,
		AnimationType animType,
		Action<WtqRect> move)
	{
		Guard.Against.Null(from);
		Guard.Against.Null(to);
		Guard.Against.Null(move);

		_log.LogInformation("Tweening from '{From}' to '{To}' in '{Duration}'ms, with animation type '{AnimationType}'", from, to, durationMs, animType);

		var swTotal = Stopwatch.StartNew();
		var swFrame = Stopwatch.StartNew();
		var animFunc = GetAnimationFunction(animType);

		while (swTotal.ElapsedMilliseconds < durationMs)
		{
			swFrame.Restart();

			var sinceStartMs = (float)swTotal.ElapsedMilliseconds;

			var linearProgress = sinceStartMs / durationMs;
			var progress = (float)animFunc(linearProgress);

			var rect = WtqRect.Lerp(from, to, progress);

			move(rect);

			// Wait for the frame to end.
			var waitMs = FrameTimeMs - swFrame.ElapsedMilliseconds;
			if (waitMs > 0)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(waitMs)).ConfigureAwait(false);
			}
		}

		// To ensure we end up in exactly the correct final position.
		move(to);

		// TODO
		_log.LogInformation("Moved window to {Bounds}", to);
	}

	private Func<double, double> GetAnimationFunction(AnimationType type)
	{
		switch (type)
		{
			case AnimationType.Linear:
				return x => x;

			case AnimationType.EaseInCubic:
				return x => Math.Pow(x, 3);

			case AnimationType.EaseOutCubic:
				return x => 1.0 - Math.Pow(1.0 - x, 3);

			case AnimationType.EaseInOutSine:
				return x => -(Math.Cos(Math.PI * x) - 1.0) / 2.0;

			case AnimationType.EaseInQuart:
				return x => Math.Pow(x, 4);

			case AnimationType.EaseOutQuart:
				return x => 1.0 - Math.Pow(1.0 - x, 4);

			case AnimationType.EaseInBack:
				return x => (2.70158 * x * x * x) - (1.70158 * x * x);

			case AnimationType.EaseOutBack:
				return x => 1.0 + (2.70158 * Math.Pow(x - 1.0, 3)) + (1.70158 * Math.Pow(x - 1.0, 2));

			default:
				_log.LogWarning("Invalid animation type '{Type}' falling back to linear", type);
				return x => x;
		}
	}
}