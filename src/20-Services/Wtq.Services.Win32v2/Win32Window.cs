namespace Wtq.Services.Win32v2;

public class Win32Window(Process process)
{
	public bool IsMainWindow { get; set; }

	public Process Process { get; } = Guard.Against.Null(process);

	public uint ProcessId { get; set; }

	public Rectangle Rect { get; set; }

	public nint Style { get; set; }

	public uint ThreadId { get; set; }

	public string? WindowCaption { get; set; }

	public string? WindowClass { get; set; }

	public nint WindowHandle { get; set; }
}