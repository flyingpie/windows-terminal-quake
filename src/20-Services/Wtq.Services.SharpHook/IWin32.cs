namespace Wtq.Services.SharpHook;

/// <summary>
/// Wrapper for talking to Win32 apis.
/// </summary>
public interface IWin32
{
	/// <summary>
	/// Returns whether the ALT key is currently held down.
	/// </summary>
	bool IsAltPressed();

	/// <summary>
	/// Returns whether the CONTROL key is currently held down.
	/// </summary>
	bool IsControlPressed();

	/// <summary>
	/// Returns whether the SHIFT key is currently held down.
	/// </summary>
	bool IsShiftPressed();

	/// <summary>
	/// Returns whether the SUPER (or "Meta" or "Windows") key is currently held down.
	/// </summary>
	bool IsSuperPressed();

	/// <summary>
	/// Attempts to convert the specified virtual <paramref name="keyCode"/> to a UTF8 character representation,
	/// taking the current keyboard layout into account.
	/// </summary>
	string? KeyCodeToKeyChar(uint keyCode);
}