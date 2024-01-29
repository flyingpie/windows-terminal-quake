namespace Wtq.Core.Data;

[Flags]
public enum WtqKeyModifiers
{
	None = 0,

	Alt = 1,
	Control = 2,
	Shift = 4,
	Super = 8,
	NoRepeat = 0x4000,
}