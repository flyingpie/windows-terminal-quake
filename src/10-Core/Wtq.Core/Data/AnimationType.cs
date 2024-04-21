namespace Wtq.Core.Data;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: Self-explanatory.")]
public enum AnimationType
{
	Linear = 0,

	EaseInBack,
	EaseInCubic,
	EaseInOutSine,
	EaseInQuart,
	EaseOutBack,
	EaseOutCubic,
	EaseOutQuart,
}