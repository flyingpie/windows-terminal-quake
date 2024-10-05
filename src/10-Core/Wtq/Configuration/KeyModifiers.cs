namespace Wtq.Configuration;

/// <summary>
/// Alt, control, etc..
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
	/// Control (either left or right).
	/// </summary>
	Control = 2,

	/// <summary>
	/// Shift.
	/// </summary>
	Shift = 4,

	/// <summary>
	/// The "Windows" key on many keyboards.
	/// </summary>
	Super = 8,

	/// <summary>
	/// Prevents holding down the key-combo from firing multiple events.
	/// TODO: Maybe move this out of the generic lib and into the Win32 or WinForms one.
	/// </summary>
	NoRepeat = 0x4000,
}