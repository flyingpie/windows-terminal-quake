namespace Wtq.Services.AnimationTypeProviders;

public class AnimationProvider : IAnimationProvider
{
	private readonly ILogger _log = Log.For<AnimationProvider>();

	/// <inheritdoc/>
	public Func<double, double> GetAnimationFunction()
	{
		return GetAnimationFunction(AnimationType.EaseOutQuart);
	}

	/// <inheritdoc/>
	public Func<double, double> GetAnimationFunction(AnimationType type)
	{
		switch (type)
		{
			case AnimationType.Linear:
				return (x) => x;

			case AnimationType.EaseInCubic:
				return (x) => Math.Pow(x, 3);

			case AnimationType.EaseOutCubic:
				return (x) => 1.0 - Math.Pow(1.0 - x, 3);

			case AnimationType.EaseInOutSine:
				return (x) => -(Math.Cos(Math.PI * x) - 1.0) / 2.0;

			case AnimationType.EaseInQuart:
				return (x) => Math.Pow(x, 4);

			case AnimationType.EaseOutQuart:
				return (x) => 1.0 - Math.Pow(1.0 - x, 4);

			case AnimationType.EaseInBack:
				return (x) => 2.70158 * x * x * x - 1.70158 * x * x;

			case AnimationType.EaseOutBack:
				return (x) => 1.0 + 2.70158 * Math.Pow(x - 1.0, 3) + 1.70158 * Math.Pow(x - 1.0, 2);

			default:
				_log.LogWarning("Invalid animation type '{Type}' falling back to linear", type);
				return (x) => x;
		}
	}
}