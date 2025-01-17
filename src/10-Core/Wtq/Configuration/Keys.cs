namespace Wtq.Configuration;

[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "MvdO: Some overlap for convenience.")]
public enum Keys
{
	/// <summary>
	/// The bitmask to extract a key code from a key value.
	/// </summary>
	KeyCode = 0xFFFF,

	/// <summary>
	/// The bitmask to extract modifiers from a key value.
	/// </summary>
	Modifiers = -65536,

	/// <summary>
	/// No key pressed.
	/// </summary>
	None = 0,

	/// <summary>
	/// The left mouse button.
	/// </summary>
	LButton = 1,

	/// <summary>
	/// The right mouse button.
	/// </summary>
	RButton = 2,

	/// <summary>
	/// The CANCEL key.
	/// </summary>
	Cancel = 3,

	/// <summary>
	/// The middle mouse button (three-button mouse).
	/// </summary>
	MButton = 4,

	/// <summary>
	/// The first x mouse button (five-button mouse).
	/// </summary>
	XButton1 = 5,

	/// <summary>
	/// The second x mouse button (five-button mouse).
	/// </summary>
	XButton2 = 6,

	/// <summary>
	/// The BACKSPACE key.
	/// </summary>
	[Display(Description = "Backspace")]
	Back = 8,

	/// <summary>
	/// The TAB key.
	/// </summary>
	[Display(Description = "Tab")]
	Tab = 9,

	/// <summary>
	/// The LINEFEED key.
	/// </summary>
	LineFeed = 0xA,

	/// <summary>
	/// The CLEAR key.
	/// </summary>
	Clear = 0xC,

	/// <summary>
	/// The RETURN key.
	/// </summary>
	[Display(Description = "Return (enter)")]
	Return = 0xD,

	/// <summary>
	/// The ENTER key.
	/// </summary>
	[Display(Description = "Return (enter)")]
	Enter = 0xD,

	/// <summary>
	/// The SHIFT key.
	/// </summary>
	ShiftKey = 0x10,

	/// <summary>
	/// The CTRL key.
	/// </summary>
	ControlKey = 0x11,

	/// <summary>
	/// The ALT key.
	/// </summary>
	Menu = 0x12,

	/// <summary>
	/// The PAUSE key.
	/// </summary>
	Pause = 0x13,

	/// <summary>
	/// The CAPS LOCK key.
	/// </summary>
	[Display(Description = "Caps lock")]
	CapsLock = 0x14,

	/// <summary>
	/// The ESC key.
	/// </summary>
	[Display(Description = "Escape")]
	Escape = 0x1B,

	/// <summary>
	/// The SPACEBAR key.
	/// </summary>
	[Display(Description = "Space bar")]
	Space = 0x20,

	/// <summary>
	/// The PAGE UP key.
	/// </summary>
	[Display(Description = "Page up")]
	PageUp = 0x21,

	/// <summary>
	/// The PAGE DOWN key.
	/// </summary>
	[Display(Description = "Page down")]
	PageDown = 0x22,

	/// <summary>
	/// The END key.
	/// </summary>
	[Display(Description = "End")]
	End = 0x23,

	/// <summary>
	/// The HOME key.
	/// </summary>
	[Display(Description = "Home")]
	Home = 0x24,

	/// <summary>
	/// The LEFT ARROW key.
	/// </summary>
	[Display(Description = "Left arrow")]
	Left = 0x25,

	/// <summary>
	/// The UP ARROW key.
	/// </summary>
	[Display(Description = "Up arrow")]
	Up = 0x26,

	/// <summary>
	/// The RIGHT ARROW key.
	/// </summary>
	[Display(Description = "Right arrow")]
	Right = 0x27,

	/// <summary>
	/// The DOWN ARROW key.
	/// </summary>
	[Display(Description = "Down arrow")]
	Down = 0x28,

	/// <summary>
	/// The SELECT key.
	/// </summary>
	Select = 0x29,

	/// <summary>
	/// The PRINT key.
	/// </summary>
	Print = 0x2A,

	/// <summary>
	/// The EXECUTE key.
	/// </summary>
	Execute = 0x2B,

	/// <summary>
	/// The PRINT SCREEN key.
	/// </summary>
	[Display(Description = "Print screen")]
	PrintScreen = 0x2C,

	/// <summary>
	/// The INS key.
	/// </summary>
	[Display(Description = "Insert")]
	Insert = 0x2D,

	/// <summary>
	/// The DEL key.
	/// </summary>
	[Display(Description = "Delete")]
	Delete = 0x2E,

	/// <summary>
	/// The HELP key.
	/// </summary>
	Help = 0x2F,

	/// <summary>
	/// The 0 key.
	/// </summary>
	[Display(Description = "0")]
	D0 = 0x30,

	/// <summary>
	/// The 1 key.
	/// </summary>
	[Display(Description = "1")]
	D1 = 0x31,

	/// <summary>
	/// The 2 key.
	/// </summary>
	[Display(Description = "2")]
	D2 = 0x32,

	/// <summary>
	/// The 3 key.
	/// </summary>
	[Display(Description = "3")]
	D3 = 0x33,

	/// <summary>
	/// The 4 key.
	/// </summary>
	[Display(Description = "4")]
	D4 = 0x34,

	/// <summary>
	/// The 5 key.
	/// </summary>
	[Display(Description = "5")]
	D5 = 0x35,

	/// <summary>
	/// The 6 key.
	/// </summary>
	[Display(Description = "6")]
	D6 = 0x36,

	/// <summary>
	/// The 7 key.
	/// </summary>
	[Display(Description = "7")]
	D7 = 0x37,

	/// <summary>
	/// The 8 key.
	/// </summary>
	[Display(Description = "8")]
	D8 = 0x38,

	/// <summary>
	/// The 9 key.
	/// </summary>
	[Display(Description = "9")]
	D9 = 0x39,

	/// <summary>
	/// The A key.
	/// </summary>
	[Display(Description = "A")]
	A = 0x41,

	/// <summary>
	/// The B key.
	/// </summary>
	[Display(Description = "B")]
	B = 0x42,

	/// <summary>
	/// The C key.
	/// </summary>
	[Display(Description = "C")]
	C = 0x43,

	/// <summary>
	/// The D key.
	/// </summary>
	[Display(Description = "D")]
	D = 0x44,

	/// <summary>
	/// The E key.
	/// </summary>
	[Display(Description = "E")]
	E = 0x45,

	/// <summary>
	/// The F key.
	/// </summary>
	[Display(Description = "F")]
	F = 0x46,

	/// <summary>
	/// The G key.
	/// </summary>
	[Display(Description = "G")]
	G = 0x47,

	/// <summary>
	/// The H key.
	/// </summary>
	[Display(Description = "H")]
	H = 0x48,

	/// <summary>
	/// The I key.
	/// </summary>
	[Display(Description = "I")]
	I = 0x49,

	/// <summary>
	/// The J key.
	/// </summary>
	[Display(Description = "J")]
	J = 0x4A,

	/// <summary>
	/// The K key.
	/// </summary>
	[Display(Description = "K")]
	K = 0x4B,

	/// <summary>
	/// The L key.
	/// </summary>
	[Display(Description = "L")]
	L = 0x4C,

	/// <summary>
	/// The M key.
	/// </summary>
	[Display(Description = "M")]
	M = 0x4D,

	/// <summary>
	/// The N key.
	/// </summary>
	[Display(Description = "N")]
	N = 0x4E,

	/// <summary>
	/// The O key.
	/// </summary>
	[Display(Description = "O")]
	O = 0x4F,

	/// <summary>
	/// The P key.
	/// </summary>
	[Display(Description = "P")]
	P = 0x50,

	/// <summary>
	/// The Q key.
	/// </summary>
	[Display(Description = "Q")]
	Q = 0x51,

	/// <summary>
	/// The R key.
	/// </summary>
	[Display(Description = "R")]
	R = 0x52,

	/// <summary>
	/// The S key.
	/// </summary>
	[Display(Description = "S")]
	S = 0x53,

	/// <summary>
	/// The T key.
	/// </summary>
	[Display(Description = "T")]
	T = 0x54,

	/// <summary>
	/// The U key.
	/// </summary>
	[Display(Description = "U")]
	U = 0x55,

	/// <summary>
	/// The V key.
	/// </summary>
	[Display(Description = "V")]
	V = 0x56,

	/// <summary>
	/// The W key.
	/// </summary>
	[Display(Description = "W")]
	W = 0x57,

	/// <summary>
	/// The X key.
	/// </summary>
	[Display(Description = "X")]
	X = 0x58,

	/// <summary>
	/// The Y key.
	/// </summary>
	[Display(Description = "Y")]
	Y = 0x59,

	/// <summary>
	/// The Z key.
	/// </summary>
	[Display(Description = "Z")]
	Z = 0x5A,

	/// <summary>
	/// The left Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	[Display(Description = "Left super")]
	LWin = 0x5B,

	/// <summary>
	/// The right Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	[Display(Description = "Right super")]
	RWin = 0x5C,

	/// <summary>
	/// The application key (Microsoft Natural Keyboard).
	/// </summary>
	Apps = 0x5D,

	/// <summary>
	/// The computer sleep key.
	/// </summary>
	Sleep = 0x5F,

	/// <summary>
	/// The 0 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 0")]
	NumPad0 = 0x60,

	/// <summary>
	/// The 1 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 1")]
	NumPad1 = 0x61,

	/// <summary>
	/// The 2 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 2")]
	NumPad2 = 0x62,

	/// <summary>
	/// The 3 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 3")]
	NumPad3 = 0x63,

	/// <summary>
	/// The 4 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 4")]
	NumPad4 = 0x64,

	/// <summary>
	/// The 5 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 5")]
	NumPad5 = 0x65,

	/// <summary>
	/// The 6 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 6")]
	NumPad6 = 0x66,

	/// <summary>
	/// The 7 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 7")]
	NumPad7 = 0x67,

	/// <summary>
	/// The 8 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 8")]
	NumPad8 = 0x68,

	/// <summary>
	/// The 9 key on the numeric keypad.
	/// </summary>
	[Display(Description = "Numpad 9")]
	NumPad9 = 0x69,

	/// <summary>
	/// The multiply key.
	/// </summary>
	[Display(Description = "Numpad *")]
	Multiply = 0x6A,

	/// <summary>
	/// The add key.
	/// </summary>
	[Display(Description = "Numpad +")]
	Add = 0x6B,

	/// <summary>
	/// The separator key.
	/// </summary>
	Separator = 0x6C,

	/// <summary>
	/// The subtract key.
	/// </summary>
	[Display(Description = "Numpad -")]
	Subtract = 0x6D,

	/// <summary>
	/// The decimal key.
	/// </summary>
#pragma warning disable CA1720 // Identifier contains type name
	Decimal = 0x6E,
#pragma warning restore CA1720 // Identifier contains type name

	/// <summary>
	/// The divide key.
	/// </summary>
	[Display(Description = "Numpad /")]
	Divide = 0x6F,

	/// <summary>
	/// The F1 key.
	/// </summary>
	[Display(Description = "F1")]
	F1 = 0x70,

	/// <summary>
	/// The F2 key.
	/// </summary>
	[Display(Description = "F2")]
	F2 = 0x71,

	/// <summary>
	/// The F3 key.
	/// </summary>
	[Display(Description = "F3")]
	F3 = 0x72,

	/// <summary>
	/// The F4 key.
	/// </summary>
	[Display(Description = "F4")]
	F4 = 0x73,

	/// <summary>
	/// The F5 key.
	/// </summary>
	[Display(Description = "F5")]
	F5 = 0x74,

	/// <summary>
	/// The F6 key.
	/// </summary>
	[Display(Description = "F6")]
	F6 = 0x75,

	/// <summary>
	/// The F7 key.
	/// </summary>
	[Display(Description = "F7")]
	F7 = 0x76,

	/// <summary>
	/// The F8 key.
	/// </summary>
	[Display(Description = "F8")]
	F8 = 0x77,

	/// <summary>
	/// The F9 key.
	/// </summary>
	[Display(Description = "F9")]
	F9 = 0x78,

	/// <summary>
	/// The F10 key.
	/// </summary>
	[Display(Description = "F10")]
	F10 = 0x79,

	/// <summary>
	/// The F11 key.
	/// </summary>
	[Display(Description = "F11")]
	F11 = 0x7A,

	/// <summary>
	/// The F12 key.
	/// </summary>
	[Display(Description = "F12")]
	F12 = 0x7B,

	/// <summary>
	/// The F13 key.
	/// </summary>
	F13 = 0x7C,

	/// <summary>
	/// The F14 key.
	/// </summary>
	F14 = 0x7D,

	/// <summary>
	/// The F15 key.
	/// </summary>
	F15 = 0x7E,

	/// <summary>
	/// The F16 key.
	/// </summary>
	F16 = 0x7F,

	/// <summary>
	/// The F17 key.
	/// </summary>
	F17 = 0x80,

	/// <summary>
	/// The F18 key.
	/// </summary>
	F18 = 0x81,

	/// <summary>
	/// The F19 key.
	/// </summary>
	F19 = 0x82,

	/// <summary>
	/// The F20 key.
	/// </summary>
	F20 = 0x83,

	/// <summary>
	/// The F21 key.
	/// </summary>
	F21 = 0x84,

	/// <summary>
	/// The F22 key.
	/// </summary>
	F22 = 0x85,

	/// <summary>
	/// The F23 key.
	/// </summary>
	F23 = 0x86,

	/// <summary>
	/// The F24 key.
	/// </summary>
	F24 = 0x87,

	/// <summary>
	/// The NUM LOCK key.
	/// </summary>
	[Display(Description = "Num lock")]
	NumLock = 0x90,

	/// <summary>
	/// The SCROLL LOCK key.
	/// </summary>
	[Display(Description = "Scroll lock")]
	Scroll = 0x91,

	/// <summary>
	/// The left SHIFT key.
	/// </summary>
	LShiftKey = 0xA0,

	/// <summary>
	/// The right SHIFT key.
	/// </summary>
	RShiftKey = 0xA1,

	/// <summary>
	/// The left CTRL key.
	/// </summary>
	LControlKey = 0xA2,

	/// <summary>
	/// The right CTRL key.
	/// </summary>
	RControlKey = 0xA3,

	/// <summary>
	/// The left ALT key.
	/// </summary>
	LMenu = 0xA4,

	/// <summary>
	/// The right ALT key.
	/// </summary>
	RMenu = 0xA5,

	/// <summary>
	/// The browser back key (Windows 2000 or later).
	/// </summary>
	BrowserBack = 0xA6,

	/// <summary>
	/// The browser forward key (Windows 2000 or later).
	/// </summary>
	BrowserForward = 0xA7,

	/// <summary>
	/// The browser refresh key (Windows 2000 or later).
	/// </summary>
	BrowserRefresh = 0xA8,

	/// <summary>
	/// The browser stop key (Windows 2000 or later).
	/// </summary>
	BrowserStop = 0xA9,

	/// <summary>
	/// The browser search key (Windows 2000 or later).
	/// </summary>
	BrowserSearch = 0xAA,

	/// <summary>
	/// The browser favorites key (Windows 2000 or later).
	/// </summary>
	BrowserFavorites = 0xAB,

	/// <summary>
	/// The browser home key (Windows 2000 or later).
	/// </summary>
	BrowserHome = 0xAC,

	/// <summary>
	/// The volume mute key (Windows 2000 or later).
	/// </summary>
	VolumeMute = 0xAD,

	/// <summary>
	/// The volume down key (Windows 2000 or later).
	/// </summary>
	VolumeDown = 0xAE,

	/// <summary>
	/// The volume up key (Windows 2000 or later).
	/// </summary>
	VolumeUp = 0xAF,

	/// <summary>
	/// The media next track key (Windows 2000 or later).
	/// </summary>
	MediaNextTrack = 0xB0,

	/// <summary>
	/// The media previous track key (Windows 2000 or later).
	/// </summary>
	MediaPreviousTrack = 0xB1,

	/// <summary>
	/// The media Stop key (Windows 2000 or later).
	/// </summary>
	MediaStop = 0xB2,

	/// <summary>
	/// The media play pause key (Windows 2000 or later).
	/// </summary>
	MediaPlayPause = 0xB3,

	/// <summary>
	/// The launch mail key (Windows 2000 or later).
	/// </summary>
	LaunchMail = 0xB4,

	/// <summary>
	/// The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "; (semicolon)")]
	OemSemicolon = 0xBA,

	/// <summary>
	/// The OEM plus key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "+ (plus)")]
	Oemplus = 0xBB,

	/// <summary>
	/// The OEM comma key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = ", (comma)")]
	Oemcomma = 0xBC,

	/// <summary>
	/// The OEM minus key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "- (minus)")]
	OemMinus = 0xBD,

	/// <summary>
	/// The OEM period key on any country/region keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = ". (period)")]
	OemPeriod = 0xBE,

	/// <summary>
	/// The OEM question mark key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "? (question mark)")]
	OemQuestion = 0xBF,

	/// <summary>
	/// The OEM tilde key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "~ (tilde)")]
	Oemtilde = 0xC0,

	/// <summary>
	/// The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "[ (open bracket)")]
	OemOpenBrackets = 0xDB,

	/// <summary>
	/// The OEM pipe key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "| (pipe)")]
	OemPipe = 0xDC,

	/// <summary>
	/// The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "] (close bracket)")]
	OemCloseBrackets = 0xDD,

	/// <summary>
	/// The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
	/// </summary>
	[Display(Description = "\" (double quote)")]
	OemQuotes = 0xDE,

	/// <summary>
	/// The OEM 8 key.
	/// </summary>
	Oem8 = 0xDF,

	/// <summary>
	/// The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000.
	/// </summary>
	/// <summary>
	/// or later).
	/// </summary>
	[Display(Description = "\\ (backslash)")]
	OemBackslash = 0xE2,

	/// <summary>
	/// The PLAY key.
	/// </summary>
	Play = 0xFA,

	/// <summary>
	/// The ZOOM key.
	/// </summary>
	Zoom = 0xFB,

	/// <summary>
	/// The CLEAR key.
	/// </summary>
	OemClear = 0xFE,
}