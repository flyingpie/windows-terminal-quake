using System.Runtime.InteropServices;

namespace Wtq.WinForms.Native;

internal static class User32
{
	public const int GWL_EX_STYLE = -20;

	public const int LWA_ALPHA = 0x2;

	public const int WS_EX_APPWINDOW = 0x00040000;
	public const int WS_EX_LAYERED = 0x80000;
	public const int WS_EX_TOOLWINDOW = 0x00000080;

	public static readonly nint HWND_TOPMOST = -1;
	public const uint SWP_NOSIZE = 0x0001;
	public const uint SWP_NOMOVE = 0x0002;
	public const uint TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

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

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool UnregisterHotKey(nint hWnd, int id);
}