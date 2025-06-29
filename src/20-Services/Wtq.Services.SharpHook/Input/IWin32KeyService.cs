namespace Wtq.Services.SharpHook.Input;

/// <summary>
/// Handles converting virtual key codes to <see cref="KeySequence"/>s.
/// </summary>
public interface IWin32KeyService
{
	/// <summary>
	/// Converts the specified key code to a <see cref="KeySequence"/>.<br/>
	/// Includes attempting to resolve the key code to a (keyboard layout dependent) key character.
	/// </summary>
	KeySequence GetKeySequence(KeyCode keyCode, ushort rawKeyCode);
}