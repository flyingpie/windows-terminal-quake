namespace Wtq.Services.Win32v2;

public interface IWin32
{
	uint? GetForegroundProcessId();

	Rectangle GetWindowRect(nint windowHandle);

	string? GetWindowTitle(nint windowHandle);

	void MoveWindow(nint windowHandle, Rectangle rectangle);

	List<Win32Window> GetWindows();

	void SetAlwaysOnTop(nint windowHandle, bool isAlwaysOnTop);

	void SetForegroundWindow(nint windowHandle);

	void SetWindowTitle(nint windowHandle, string title);

	void SetWindowTransparency(nint windowHandle, int transparency);
}