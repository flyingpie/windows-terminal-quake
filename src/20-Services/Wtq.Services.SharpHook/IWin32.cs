using WKC = Wtq.Input.KeyCode;

namespace Wtq.Services.SharpHook;

/// <summary>
/// Wrapper for talking to Win32 apis.
/// </summary>
public interface IWin32
{
	/// <summary>
	/// Returns the set of <see cref="KeyModifiers"/> that are currently active.<br/>
	/// Also includes the <see cref="KeyModifiers.Numpad"/> modifier, if the specified <paramref name="keyCode"/> contains a numpad key.
	/// </summary>
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
}