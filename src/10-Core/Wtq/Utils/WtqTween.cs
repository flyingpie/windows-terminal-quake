namespace Wtq.Utils;

/// <inheritdoc/>
public sealed class WtqTween(
	IOptionsMonitor<WtqOptions> opts)
	: IWtqTween
{
	private readonly ILogger _log = Log.For<WtqTween>();
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);

	/// <inheritdoc/>
	public async Task AnimateAsync(
		Rectangle src,
		Rectangle dst,
		int durationMs,
		AnimationType animType,
		Func<Rectangle, Task> move)
	{
		Guard.Against.Null(src);
		Guard.Against.Null(dst);
		Guard.Against.Null(move);

		_log.LogInformation(
			"Tweening from {From} to {To} in {Duration}ms, with animation type {AnimationType}",
			src,
			dst,
			durationMs,
			animType);

		var swTotal = Stopwatch.StartNew();
		var swFrame = Stopwatch.StartNew();
		var animFunc = GetAnimationFunction(animType);

		var targetFps = _opts.CurrentValue.AnimationTargetFps;
		var frameTimeMs = 1000f / targetFps;

		var frameCount = 0;

		// await move(src).NoCtx();

		while (swTotal.ElapsedMilliseconds < durationMs)
		{
			frameCount++;

			swFrame.Restart();

			var sinceStartMs = (float)swTotal.ElapsedMilliseconds;

			var linearProgress = sinceStartMs / durationMs;
			var progress = (float)animFunc(linearProgress);

			var rect = MathUtils.Lerp(src, dst, progress);

			await move(rect).NoCtx();

			// Wait for the frame to end.
			var waitMs = frameTimeMs - swFrame.ElapsedMilliseconds;
			if (waitMs > 0)
			{
				await Task.Delay(TimeSpan.FromMilliseconds(waitMs)).ConfigureAwait(false);
			}
		}

		// To ensure we end up in exactly the correct final position.
		await move(dst).NoCtx();

		_log.LogInformation(
			"Tween complete, took {Actual}ms of target {Target}ms, across {FrameCount} frames",
			swTotal.ElapsedMilliseconds,
			durationMs,
			frameCount);
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