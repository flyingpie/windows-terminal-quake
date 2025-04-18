using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public class Win32Window
{
	public Guid Id { get; } = Guid.NewGuid();

	public uint ProcessId { get; set; }

	public uint ThreadId { get; set; }

	public Process Process { get; set; }

	public string WindowCaption { get; set; }

	public string WindowClass { get; set; }

	public nint WindowHandle { get; set; }

	public int Style { get; set; }

	public bool IsVisible => (Style & User32.GWLSTYLE) == User32.GWLSTYLE;

	public Rectangle Rect { get; set; }

	public Size Size => Rect.Size;
}