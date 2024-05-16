using System.Runtime.InteropServices;

namespace Wtq.Services.WinForms.Native;

internal static class User32
{
	public const int GWLEXSTYLE = -20;
	public const int LWAALPHA = 0x2;
	public const uint SWPNOMOVE = 0x0002;
	public const uint SWPNOSIZE = 0x0001;
	public const uint TOPMOSTFLAGS = SWPNOMOVE | SWPNOSIZE;
	public const int WSEXAPPWINDOW = 0x00040000;
	public const int WSEXLAYERED = 0x80000;
	public const int WSEXTOOLWINDOW = 0x00000080;

	public static readonly nint HWNDTOPMOST = -1;

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool UnregisterHotKey(nint hWnd, int id);
}