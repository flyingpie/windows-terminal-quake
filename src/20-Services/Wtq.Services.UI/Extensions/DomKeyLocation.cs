namespace Wtq.Services.UI.Extensions;

/// <summary>
/// The following constants identify which part of the keyboard the key event originates from.
/// They are accessed as KeyboardEvent.DOM_KEY_LOCATION_STANDARD and so forth.
/// </summary>
/// <remarks>https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent#keyboard_locations.</remarks>
public enum DomKeyLocation
{
	/// <summary>
	/// DOM_KEY_LOCATION_STANDARD
	///
	/// The key described by the event is not identified as being located in a particular area of the keyboard; it is not
	/// located on the numeric keypad (unless it's the NumLock key), and for keys that are duplicated on the left and right
	/// sides of the keyboard, the key is, for whatever reason, not to be associated with that location.
	///
	/// Examples include alphanumeric keys on the standard PC 101 US keyboard, the NumLock key, and the space bar.
	/// </summary>
	Standard = 0x00,

	/// <summary>
	/// DOM_KEY_LOCATION_LEFT
	///
	/// The key is one which may exist in multiple locations on the keyboard and, in this instance, is on the left side
	/// of the keyboard.
	///
	/// Examples include the left Control key, the left Command key on a Macintosh keyboard, or the left Shift key.
	/// </summary>
	Left = 0x01,

	/// <summary>
	/// DOM_KEY_LOCATION_RIGHT
	///
	/// The key is one which may exist in multiple positions on the keyboard and, in this case, is located on the right
	/// side of the keyboard.
	///
	/// Examples include the right Shift key and the right Alt key (Option on a Mac keyboard).
	/// </summary>
	Right = 0x02,

	/// <summary>
	/// DOM_KEY_LOCATION_NUMPAD
	///
	/// The key is located on the numeric keypad, or is a virtual key associated with the numeric keypad if there's more
	/// than one place the key could originate from. The NumLock key does not fall into this group and is always encoded
	/// with the location DOM_KEY_LOCATION_STANDARD.
	///
	/// Examples include the digits on the numeric keypad, the keypad's Enter key, and the decimal point on the keypad.
	/// </summary>
	Numpad = 0x03,

	/// <summary>
	/// Should ever happen, but useful to detect cases where the location isn't known.
	/// </summary>
	Unknown = 0xfff,
}