using System.Runtime.InteropServices;

namespace Wtq.Services.Win32.Native;

public static class Kernel32
{
	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool AllocConsole();

	[DllImportAttribute("kernel32.dll", EntryPoint = "GetProcessId")]
	public static extern uint GetProcessId([In] System.IntPtr process);
}