namespace Wtq.Input;

/// <summary>
/// Alt, control, etc.<br/>
/// Named plurally, as it can contain multiple values.
/// </summary>
[Flags]
public enum KeyModifiers
{
	/// <summary>
	/// Used to detect serialization issues.
	/// </summary>
	None = 0,

	/// <summary>
	/// Alt.
	/// </summary>
	Alt = 1,

	/// <summary>
	/// Control.
	/// </summary>
	Control = 2,

	/// <summary>
	/// Shift.
	/// </summary>
	Shift = 4,

	/// <summary>
	/// The "Windows" key on many keyboards. Otherwise known as "Meta".
	/// </summary>
	Super = 8,

	/// <summary>
	/// Key came from the numpad/keypad. Only used in combination with key characters (KeyChars as opposed to KeyCodes), to differentiate keys
	/// that can come from either the numpad or somewhere else (like "1").<br/>
	/// Borrowed from Qt.
	/// </summary>
	Numpad = 16,

	/// <summary>
	/// Prevents holding down the key-combo from firing multiple events.
	/// TODO: Maybe move this out of the generic lib and into the Win32 or WinForms one.
	/// </summary>
	NoRepeat = 0x4000,
}