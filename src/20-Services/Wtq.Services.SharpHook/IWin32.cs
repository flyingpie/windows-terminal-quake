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
}