using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Wtq.SimpleTrayIcon.Native;

public static class User32
{
	[DllImport("User32", ExactSpelling = true, EntryPoint = "DispatchMessageW")]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe nint DispatchMessage(Msg* lpMsg);

	[DllImport("User32", ExactSpelling = true, EntryPoint = "GetMessageW", SetLastError = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe bool GetMessage(Msg* lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

	internal static unsafe void RunLoop(CancellationToken cancellationToken)
	{
		Msg msg = default;
		var lpMsg = &msg;
		while (!cancellationToken.IsCancellationRequested && GetMessage(lpMsg, default, 0, 0))
		{
			TranslateMessage(lpMsg);
			DispatchMessage(lpMsg);
		}
	}

	[DllImport("User32", ExactSpelling = true)]
	[DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
	internal static extern unsafe int TranslateMessage(Msg* lpMsg);

	[StructLayout(LayoutKind.Sequential)]
	internal struct Msg
	{
		internal nint Hwnd;
		internal uint Message;
		internal uint WParam;
		internal uint LParam;
		internal uint Time;
		internal int PtX;
		internal int PtY;
	}
}