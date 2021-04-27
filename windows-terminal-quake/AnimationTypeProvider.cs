using Serilog;
using System;

namespace WindowsTerminalQuake
{
	public interface IAnimationTypeProvider
	{
		Func<double, double> GetAnimationFunction();

		/// <summary>
		/// Returns a mathematical function that can be used for "easing" animations.
		/// Such functions are typically given an X value(representing time) between 0.0 and 1.0,
		/// and return a Y value between 0.0 and 1.0 (representing the position of what we're animating).
		/// </summary>
		/// <param name="type">Name of the easing function; we use the same names as https://easings.net/.</param>
		Func<double, double> GetAnimationFunction(AnimationType type);
	}

	public class AnimationTypeProvider : IAnimationTypeProvider
	{
		public Func<double, double> GetAnimationFunction()
		{
			return GetAnimationFunction(Settings.Instance.ToggleAnimationType);
		}

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
					Log.Warning("Invalid animation type \"" + type + "\"; falling back to linear.");
					return (x) => x;
			}
		}
	}
}