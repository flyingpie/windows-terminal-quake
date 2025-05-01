using Wtq.Services.Win32.Native;

namespace Wtq.Services.Win32;

public class Win32Window(Process process)
{
	public Guid Id { get; } = Guid.NewGuid();

	public bool IsVisible
		=> (Style & User32.GWLSTYLE) == User32.GWLSTYLE;

	public Process Process { get; } = Guard.Against.Null(process);

	public uint ProcessId { get; set; }

	public Rectangle Rect { get; set; }

	public Size Size => Rect.Size;

	public nint Style { get; set; }

	public uint ThreadId { get; set; }

	public string? WindowCaption { get; set; }

	public string? WindowClass { get; set; }

	public nint WindowHandle { get; set; }
}