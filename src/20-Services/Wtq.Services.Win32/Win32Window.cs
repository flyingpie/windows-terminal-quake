namespace Wtq.Services.Win32;

public class Win32Window(Func<bool> hasExited)
{
	private readonly Func<bool> _hasExited = Guard.Against.Null(hasExited);

	public bool HasExited => _hasExited();

	public bool IsMainWindow { get; set; }

	public nint MainWindowHandle { get; set; }

	public uint ProcessId { get; set; }

	public string? ProcessName { get; set; }

	public Rectangle Rect { get; set; }

	public nint Style { get; set; }

	public uint ThreadId { get; set; }

	public string? WindowCaption { get; set; }

	public string? WindowClass { get; set; }

	public nint WindowHandle { get; set; }

	public override string ToString() => $"ProcessId:{ProcessId} ProcessName:{ProcessName} MainWindow:{MainWindowHandle:X} WindowHandle:{WindowHandle:X}";
}