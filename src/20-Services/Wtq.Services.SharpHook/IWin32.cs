using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook;

/// <summary>
/// Wrapper for talking to Win32 apis.
/// </summary>
public interface IWin32
{
	/// <summary>
	/// Converts the specified key code to a <see cref="KeySequence"/>.<br/>
	/// Includes attempting to resolve the key code to a (keyboard layout dependent) key character.
	/// </summary>
	KeySequence GetKeySequence(KeyCode keyCode, ushort rawKeyCode);

	KeyModifiers GetModifiers(WKC keyCode);

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