using System;

namespace WindowsTerminalQuake.Native
{
	[Flags]
	public enum KeyModifiers
	{
		None = 0,

		Alt = 1,
		Control = 2,
		Shift = 4,
		Windows = 8,
		NoRepeat = 0x4000
	}
}