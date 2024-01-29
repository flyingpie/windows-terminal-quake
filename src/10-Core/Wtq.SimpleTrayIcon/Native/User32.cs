using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Wtq.SimpleTrayIcon.Native;

public static class User32
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct MSG
	{
		internal nint hwnd;
		internal uint message;
		internal uint wParam;
		internal uint lParam;
		internal uint time;
		internal int ptX;
		internal int ptY;
	}

	[DllImport("User32", ExactSpelling = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe int TranslateMessage(MSG* lpMsg);

	[DllImport("User32", ExactSpelling = true, EntryPoint = "DispatchMessageW")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe nint DispatchMessage(MSG* lpMsg);

	[DllImport("User32", ExactSpelling = true, EntryPoint = "GetMessageW", SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe bool GetMessage(MSG* lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	internal static unsafe void RunLoop(CancellationToken cancellationToken)
	{
		MSG msg = default;
		var lpMsg = &msg;
		while (!cancellationToken.IsCancellationRequested && GetMessage(lpMsg, default, 0, 0))
		{
			TranslateMessage(lpMsg);
			DispatchMessage(lpMsg);
		}
	}
}