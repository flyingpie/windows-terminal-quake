namespace Wtq.Configuration;

/// <summary>
/// See here for more information: https://easings.net/.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: See link for more information on the easing types.")]
public enum AnimationType
{
	[Display(Description = "Linear")]
	Linear = 0,

	[Display(Description = "Ease in+out (sine)")]
	EaseInOutSine,

	[Display(Description = "Ease in back")]
	EaseInBack,
	[Display(Description = "Ease in cubic")]
	EaseInCubic,
	[Display(Description = "Ease in quadratic")]
	EaseInQuart,

	[Display(Description = "Ease out back")]
	EaseOutBack,
	[Display(Description = "Ease out cubic")]
	EaseOutCubic,
	[Display(Description = "Ease out quadratic")]
	EaseOutQuart,
}