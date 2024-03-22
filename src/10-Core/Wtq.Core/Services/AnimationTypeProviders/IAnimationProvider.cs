using Wtq.Core.Data;

namespace Wtq.Services.AnimationTypeProviders;

public interface IAnimationProvider
{
	/// <summary>
	/// Returns a mathematical function that can be used for "easing" animations.
	/// Such functions are typically given an X value(representing time) between 0.0 and 1.0,
	/// and return a Y value between 0.0 and 1.0 (representing the position of what we're animating).
	/// </summary>
	Func<double, double> GetAnimationFunction();

	/// <summary>
	/// Returns a mathematical function that can be used for "easing" animations.
	/// Such functions are typically given an X value(representing time) between 0.0 and 1.0,
	/// and return a Y value between 0.0 and 1.0 (representing the position of what we're animating).
	/// </summary>
	/// <param name="type">Name of the easing function; we use the same names as https://easings.net/.</param>
	Func<double, double> GetAnimationFunction(AnimationType type);
}