namespace Wtq.Core.Data;

[Flags]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: Self-explanatory.")]
public enum WtqKeyModifiers
{
	None = 0,

	Alt = 1,
	Control = 2,
	Shift = 4,
	Super = 8,
	NoRepeat = 0x4000,
}