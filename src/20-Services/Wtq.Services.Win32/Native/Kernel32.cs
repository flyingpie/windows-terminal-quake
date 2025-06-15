using System.IO;
using System.Runtime.InteropServices;

namespace Wtq.Services.Win32.Native;

public static class Kernel32
{
	public const int ATTACH_PARENT_PROCESS = -1;
	public const int STD_OUTPUT_HANDLE = -11;
	public const int STD_ERROR_HANDLE = -12;
	public const int STD_INPUT_HANDLE = -10;

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool AttachConsole(int dwProcessId);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern IntPtr GetStdHandle(int nStdHandle);

	[DllImport("kernel32.dll", SetLastError = true)]
	static extern bool SetStdHandle(int nStdHandle, IntPtr handle);

	public static void RedirectConsoleStreams()
	{
		// StdOut & StdErr
		var stdOut = GetStdHandle(STD_OUTPUT_HANDLE);
		var safeStdOut = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdOut, false);
		var stdoutWriter = new StreamWriter(new FileStream(safeStdOut, FileAccess.Write))
		{
			AutoFlush = true,
		};

		Console.SetOut(stdoutWriter);
		Console.SetError(stdoutWriter);

		// StdIn
		var stdIn = GetStdHandle(STD_INPUT_HANDLE);
		var safeStdIn = new Microsoft.Win32.SafeHandles.SafeFileHandle(stdIn, false);
		var stdinReader = new StreamReader(new FileStream(safeStdIn, FileAccess.Read));
		Console.SetIn(stdinReader);
	}
}