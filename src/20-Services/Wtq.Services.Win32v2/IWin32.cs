namespace Wtq.Services.Win32v2;

/// <summary>
/// Provides access to Windows OS functions.
/// </summary>
public interface IWin32
{
	/// <summary>
	/// Returns the id of the window that currently has focus.<br/>
	/// Returns null if no window was found.
	/// </summary>
	nint? GetForegroundWindowHandle();

	/// <summary>
	/// Returns a <see cref="Win32Window"/> corresponding to the specified window handle.<br/>
	/// Returns null if no window could be found.
	/// </summary>
	Win32Window? GetWindow(
		nint windowHandle);

	/// <summary>
	/// Returns the class of a window (if any), used to filter windows on.
	/// </summary>
	string? GetWindowClass(
		nint windowHandle);

	/// <summary>
	/// Returns a list of windows currently active.
	/// </summary>
	ICollection<Win32Window> GetWindowList();

	/// <summary>
	/// Returns the <see cref="Rectangle"/> describing the window with the specified <paramref name="windowHandle"/>.<br/>
	/// Throws an exception if no rectangle could be obtained.
	/// </summary>
	Rectangle GetWindowRect(
		nint windowHandle);

	/// <summary>
	/// Returns the title of a window (if any), used to filter windows on.
	/// </summary>
	string? GetWindowTitle(
		nint windowHandle);

	/// <summary>
	/// Returns whether the specified window handle (still) points to a valid window.
	/// </summary>
	bool IsValidWindow(
		nint windowHandle);

	/// <summary>
	/// Sets the position and size of the window with the specified <paramref name="windowHandle"/>.
	/// </summary>
	void MoveWindow(
		nint windowHandle,
		Rectangle rectangle);

	WindowShowStyle GetWindowState(
		nint windowHandle);

	/// <summary>
	/// Toggles a window to be always-on-top.
	/// </summary>
	void SetAlwaysOnTop(
		nint windowHandle,
		bool isAlwaysOnTop);

	/// <summary>
	/// Attempts to set the foreground window. Note that this may not always work, depending on a variety of conditions.
	/// </summary>
	void SetForegroundWindow(
		nint windowHandle);

	void SetWindowState(
		nint windowHandle,
		WindowShowStyle state);

	/// <summary>
	/// Attempts to set the title of a window. Note that this may not always work, depending on what kind of window we're dealing with.
	/// </summary>
	void SetWindowTitle(
		nint windowHandle,
		string title);

	/// <summary>
	/// Attempts to set a window's transparency. Note that this may not always work, depending on the window's rendering setup.
	/// </summary>
	void SetWindowTransparency(
		nint windowHandle,
		int transparency);
}