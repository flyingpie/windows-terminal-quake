using System;

namespace WindowsTerminalQuake.Native
{
	[Flags]
	public enum KeyModifiers
	{
		Alt = 1,
		Control = 2,
		Shift = 4,
		Windows = 8,
		NoRepeat = 0x4000
	}
}