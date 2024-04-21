namespace Wtq.Services.WinForms.Native;

[Flags]
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "MvdO: Self-explanatory.")]
internal enum KeyModifiers
{
	None = 0,

	Alt = 1,
	Control = 2,
	Shift = 4,
	Windows = 8,
	NoRepeat = 0x4000,
}