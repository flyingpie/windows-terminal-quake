namespace Wtq.Services.AnimationTypeProviders;

/// <summary>
/// Defines functions that convert a linear time scale into one that includes easing (such as <see cref="AnimationType.EaseInQuart"/> or <see cref="AnimationType.EaseInCubic"/>).
/// </summary>
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