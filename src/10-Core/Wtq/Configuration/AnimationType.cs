namespace Wtq.Configuration;

/// <summary>
/// See here for more information: https://easings.net/.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: See link for more information on the easing types.")]
public enum AnimationType
{
	Linear = 0,

	EaseInOutSine,

	EaseInBack,
	EaseInCubic,
	EaseInQuart,

	EaseOutBack,
	EaseOutCubic,
	EaseOutQuart,
}