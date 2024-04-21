using System.Runtime.InteropServices;

namespace Wtq.Services.Win32.Native;

public static class Kernel32
{
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AllocConsole();
}