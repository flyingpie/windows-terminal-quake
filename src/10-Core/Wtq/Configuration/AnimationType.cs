namespace Wtq.Configuration;

/// <summary>
/// See here for more information: https://easings.net/.
/// </summary>
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: See link for more information on the easing types.")]
public enum AnimationType
{
	[Display(Name = "Linear")]
	Linear = 0,

	[Display(Name = "Ease in+out (sine)")]
	EaseInOutSine,

	[Display(Name = "Ease in back")]
	EaseInBack,
	[Display(Name = "Ease in cubic")]
	EaseInCubic,
	[Display(Name = "Ease in quadratic")]
	EaseInQuart,

	[Display(Name = "Ease out back")]
	EaseOutBack,
	[Display(Name = "Ease out cubic")]
	EaseOutCubic,
	[Display(Name = "Ease out quadratic")]
	EaseOutQuart,
}