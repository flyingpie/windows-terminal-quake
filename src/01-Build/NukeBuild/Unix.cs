using System.Runtime.InteropServices;

public static class Unix
{
	[DllImport("libc", SetLastError = true)]
	public static extern uint getuid();
}
