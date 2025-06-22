namespace Wtq.Configuration;

[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "MvdO: Some overlap for convenience.")]
[Flags]
public enum Keys
{
	/// <summary>
	/// The bitmask to extract a key code from a key value.
	/// </summary>
	KeyCode = 0x0000FFFF,

	/// <summary>
	/// The bitmask to extract modifiers from a key value.
	/// </summary>
	Modifiers = unchecked((int)0xFFFF0000),

	/// <summary>
	/// No key pressed.
	/// </summary>
	None = 0x00,

	/// <summary>
	/// The left mouse button.
	/// </summary>
	LButton = 0x01,

	/// <summary>
	/// The right mouse button.
	/// </summary>
	RButton = 0x02,

	/// <summary>
	/// The CANCEL key.
	/// </summary>
	Cancel = 0x03,

	/// <summary>
	/// The middle mouse button (three-button mouse).
	/// </summary>
	MButton = 0x04,

	/// <summary>
	/// The first x mouse button (five-button mouse).
	/// </summary>
	XButton1 = 0x05,

	/// <summary>
	/// The second x mouse button (five-button mouse).
	/// </summary>
	XButton2 = 0x06,

	/// <summary>
	/// The BACKSPACE key.
	/// </summary>
	[Display(Name = "Backspace")]
	Back = 0x08,

	/// <summary>
	/// The TAB key.
	/// </summary>
	[Display(Name = "Tab")]
	Tab = 0x09,

	/// <summary>
	/// The LINEFEED key.
	/// </summary>
	LineFeed = 0x0A,

	/// <summary>
	/// The CLEAR key.
	/// </summary>
	Clear = 0x0C,

	/// <summary>
	/// The RETURN key.
	/// </summary>
	[Display(Name = "Return (enter)")]
	Return = 0x0D,

	/// <summary>
	/// The ENTER key.
	/// </summary>
	[Display(Name = "Return (enter)")]
	Enter = Return,

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
	Capital = 0x14,

	/// <summary>
	/// The CAPS LOCK key.
	/// </summary>
	[Display(Name = "Caps lock")]
	CapsLock = 0x14,

	/// <summary>
	/// The IME Kana mode key.
	/// </summary>
	KanaMode = 0x15,

	/// <summary>
	/// The IME Hanguel mode key.
	/// </summary>
	HanguelMode = 0x15,

	/// <summary>
	/// The IME Hangul mode key.
	/// </summary>
	HangulMode = 0x15,

	/// <summary>
	/// The IME Junja mode key.
	/// </summary>
	JunjaMode = 0x17,

	/// <summary>
	/// The IME Final mode key.
	/// </summary>
	FinalMode = 0x18,

	/// <summary>
	/// The IME Hanja mode key.
	/// </summary>
	HanjaMode = 0x19,

	/// <summary>
	/// The IME Kanji mode key.
	/// </summary>
	KanjiMode = 0x19,

	/// <summary>
	/// The ESC key.
	/// </summary>
	[Display(Name = "Escape")]
	Escape = 0x1B,

	/// <summary>
	/// The IME Convert key.
	/// </summary>
	IMEConvert = 0x1C,

	/// <summary>
	/// The IME NonConvert key.
	/// </summary>
	IMENonconvert = 0x1D,

	/// <summary>
	/// The IME Accept key.
	/// </summary>
	IMEAccept = 0x1E,

	/// <summary>
	/// The IME Accept key.
	/// </summary>
	IMEAceept = IMEAccept,

	/// <summary>
	/// The IME Mode change request.
	/// </summary>
	IMEModeChange = 0x1F,

	/// <summary>
	/// The SPACEBAR key.
	/// </summary>
	[Display(Name = "Space bar")]
	Space = 0x20,

	/// <summary>
	/// The PAGE UP key.
	/// </summary>
	Prior = 0x21,

	/// <summary>
	/// The PAGE UP key.
	/// </summary>
	[Display(Name = "Page up")]
	PageUp = Prior,

	/// <summary>
	/// The PAGE DOWN key.
	/// </summary>
	Next = 0x22,

	/// <summary>
	/// The PAGE DOWN key.
	/// </summary>
	[Display(Name = "Page down")]
	PageDown = Next,

	/// <summary>
	/// The END key.
	/// </summary>
	[Display(Name = "End")]
	End = 0x23,

	/// <summary>
	/// The HOME key.
	/// </summary>
	[Display(Name = "Home")]
	Home = 0x24,

	/// <summary>
	/// The LEFT ARROW key.
	/// </summary>
	[Display(Name = "Left arrow")]
	Left = 0x25,

	/// <summary>
	/// The UP ARROW key.
	/// </summary>
	[Display(Name = "Up arrow")]
	Up = 0x26,

	/// <summary>
	/// The RIGHT ARROW key.
	/// </summary>
	[Display(Name = "Right arrow")]
	Right = 0x27,

	/// <summary>
	/// The DOWN ARROW key.
	/// </summary>
	[Display(Name = "Down arrow")]
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
	Snapshot = 0x2C,

	/// <summary>
	/// The PRINT SCREEN key.
	/// </summary>
	[Display(Name = "Print screen")]
	PrintScreen = Snapshot,

	/// <summary>
	/// The INS key.
	/// </summary>
	[Display(Name = "Insert")]
	Insert = 0x2D,

	/// <summary>
	/// The DEL key.
	/// </summary>
	[Display(Name = "Delete")]
	Delete = 0x2E,

	/// <summary>
	/// The HELP key.
	/// </summary>
	Help = 0x2F,

	/// <summary>
	/// The 0 key.
	/// </summary>
	[Display(Name = "0")]
	D0 = 0x30,

	/// <summary>
	/// The 1 key.
	/// </summary>
	[Display(Name = "1")]
	D1 = 0x31,

	/// <summary>
	/// The 2 key.
	/// </summary>
	[Display(Name = "2")]
	D2 = 0x32,

	/// <summary>
	/// The 3 key.
	/// </summary>
	[Display(Name = "3")]
	D3 = 0x33,

	/// <summary>
	/// The 4 key.
	/// </summary>
	[Display(Name = "4")]
	D4 = 0x34,

	/// <summary>
	/// The 5 key.
	/// </summary>
	[Display(Name = "5")]
	D5 = 0x35,

	/// <summary>
	/// The 6 key.
	/// </summary>
	[Display(Name = "6")]
	D6 = 0x36,

	/// <summary>
	/// The 7 key.
	/// </summary>
	[Display(Name = "7")]
	D7 = 0x37,

	/// <summary>
	/// The 8 key.
	/// </summary>
	[Display(Name = "8")]
	D8 = 0x38,

	/// <summary>
	/// The 9 key.
	/// </summary>
	[Display(Name = "9")]
	D9 = 0x39,

	/// <summary>
	/// The A key.
	/// </summary>
	[Display(Name = "A")]
	A = 0x41,

	/// <summary>
	/// The B key.
	/// </summary>
	[Display(Name = "B")]
	B = 0x42,

	/// <summary>
	/// The C key.
	/// </summary>
	[Display(Name = "C")]
	C = 0x43,

	/// <summary>
	/// The D key.
	/// </summary>
	[Display(Name = "D")]
	D = 0x44,

	/// <summary>
	/// The E key.
	/// </summary>
	[Display(Name = "E")]
	E = 0x45,

	/// <summary>
	/// The F key.
	/// </summary>
	[Display(Name = "F")]
	F = 0x46,

	/// <summary>
	/// The G key.
	/// </summary>
	[Display(Name = "G")]
	G = 0x47,

	/// <summary>
	/// The H key.
	/// </summary>
	[Display(Name = "H")]
	H = 0x48,

	/// <summary>
	/// The I key.
	/// </summary>
	[Display(Name = "I")]
	I = 0x49,

	/// <summary>
	/// The J key.
	/// </summary>
	[Display(Name = "J")]
	J = 0x4A,

	/// <summary>
	/// The K key.
	/// </summary>
	[Display(Name = "K")]
	K = 0x4B,

	/// <summary>
	/// The L key.
	/// </summary>
	[Display(Name = "L")]
	L = 0x4C,

	/// <summary>
	/// The M key.
	/// </summary>
	[Display(Name = "M")]
	M = 0x4D,

	/// <summary>
	/// The N key.
	/// </summary>
	[Display(Name = "N")]
	N = 0x4E,

	/// <summary>
	/// The O key.
	/// </summary>
	[Display(Name = "O")]
	O = 0x4F,

	/// <summary>
	/// The P key.
	/// </summary>
	[Display(Name = "P")]
	P = 0x50,

	/// <summary>
	/// The Q key.
	/// </summary>
	[Display(Name = "Q")]
	Q = 0x51,

	/// <summary>
	/// The R key.
	/// </summary>
	[Display(Name = "R")]
	R = 0x52,

	/// <summary>
	/// The S key.
	/// </summary>
	[Display(Name = "S")]
	S = 0x53,

	/// <summary>
	/// The T key.
	/// </summary>
	[Display(Name = "T")]
	T = 0x54,

	/// <summary>
	/// The U key.
	/// </summary>
	[Display(Name = "U")]
	U = 0x55,

	/// <summary>
	/// The V key.
	/// </summary>
	[Display(Name = "V")]
	V = 0x56,

	/// <summary>
	/// The W key.
	/// </summary>
	[Display(Name = "W")]
	W = 0x57,

	/// <summary>
	/// The X key.
	/// </summary>
	[Display(Name = "X")]
	X = 0x58,

	/// <summary>
	/// The Y key.
	/// </summary>
	[Display(Name = "Y")]
	Y = 0x59,

	/// <summary>
	/// The Z key.
	/// </summary>
	[Display(Name = "Z")]
	Z = 0x5A,

	/// <summary>
	/// The left Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	[Display(Name = "Left super")]
	LWin = 0x5B,

	/// <summary>
	/// The left Windows logo key (also called "super" or "meta").
	/// </summary>
	[Display(Name = "Left super")]
	LSuperKey = 0x5B,

	/// <summary>
	/// The right Windows logo key (Microsoft Natural Keyboard).
	/// </summary>
	[Display(Name = "Right super")]
	RWin = 0x5C,

	/// <summary>
	/// The right Windows logo key (also called "super" or "meta").
	/// </summary>
	[Display(Name = "Right super")]
	RSuperKey = 0x5C,

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
	[Display(Name = "Numpad 0")]
	NumPad0 = 0x60,

	/// <summary>
	/// The 1 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 1")]
	NumPad1 = 0x61,

	/// <summary>
	/// The 2 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 2")]
	NumPad2 = 0x62,

	/// <summary>
	/// The 3 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 3")]
	NumPad3 = 0x63,

	/// <summary>
	/// The 4 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 4")]
	NumPad4 = 0x64,

	/// <summary>
	/// The 5 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 5")]
	NumPad5 = 0x65,

	/// <summary>
	/// The 6 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 6")]
	NumPad6 = 0x66,

	/// <summary>
	/// The 7 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 7")]
	NumPad7 = 0x67,

	/// <summary>
	/// The 8 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 8")]
	NumPad8 = 0x68,

	/// <summary>
	/// The 9 key on the numeric keypad.
	/// </summary>
	[Display(Name = "Numpad 9")]
	NumPad9 = 0x69,

	/// <summary>
	/// The multiply key.
	/// </summary>
	[Display(Name = "Numpad *")]
	Multiply = 0x6A,

	/// <summary>
	/// The add key.
	/// </summary>
	[Display(Name = "Numpad +")]
	Add = 0x6B,

	/// <summary>
	/// The separator key.
	/// </summary>
	Separator = 0x6C,

	/// <summary>
	/// The subtract key.
	/// </summary>
	[Display(Name = "Numpad -")]
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
	[Display(Name = "Numpad /")]
	Divide = 0x6F,

	/// <summary>
	/// The F1 key.
	/// </summary>
	[Display(Name = "F1")]
	F1 = 0x70,

	/// <summary>
	/// The F2 key.
	/// </summary>
	[Display(Name = "F2")]
	F2 = 0x71,

	/// <summary>
	/// The F3 key.
	/// </summary>
	[Display(Name = "F3")]
	F3 = 0x72,

	/// <summary>
	/// The F4 key.
	/// </summary>
	[Display(Name = "F4")]
	F4 = 0x73,

	/// <summary>
	/// The F5 key.
	/// </summary>
	[Display(Name = "F5")]
	F5 = 0x74,

	/// <summary>
	/// The F6 key.
	/// </summary>
	[Display(Name = "F6")]
	F6 = 0x75,

	/// <summary>
	/// The F7 key.
	/// </summary>
	[Display(Name = "F7")]
	F7 = 0x76,

	/// <summary>
	/// The F8 key.
	/// </summary>
	[Display(Name = "F8")]
	F8 = 0x77,

	/// <summary>
	/// The F9 key.
	/// </summary>
	[Display(Name = "F9")]
	F9 = 0x78,

	/// <summary>
	/// The F10 key.
	/// </summary>
	[Display(Name = "F10")]
	F10 = 0x79,

	/// <summary>
	/// The F11 key.
	/// </summary>
	[Display(Name = "F11")]
	F11 = 0x7A,

	/// <summary>
	/// The F12 key.
	/// </summary>
	[Display(Name = "F12")]
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
	[Display(Name = "Num lock")]
	NumLock = 0x90,

	/// <summary>
	/// The SCROLL LOCK key.
	/// </summary>
	[Display(Name = "Scroll lock")]
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
	/// The left ALT key.
	/// </summary>
	LAltKey = 0xA4,

	/// <summary>
	/// The right ALT key.
	/// </summary>
	RMenu = 0xA5,

	/// <summary>
	/// The right ALT key.
	/// </summary>
	RAltKey = 0xA5,

	/// <summary>
	/// The browser back key.
	/// </summary>
	BrowserBack = 0xA6,

	/// <summary>
	/// The browser forward key.
	/// </summary>
	BrowserForward = 0xA7,

	/// <summary>
	/// The browser refresh key.
	/// </summary>
	BrowserRefresh = 0xA8,

	/// <summary>
	/// The browser stop key.
	/// </summary>
	BrowserStop = 0xA9,

	/// <summary>
	/// The browser search key.
	/// </summary>
	BrowserSearch = 0xAA,

	/// <summary>
	/// The browser favorites key.
	/// </summary>
	BrowserFavorites = 0xAB,

	/// <summary>
	/// The browser home key.
	/// </summary>
	BrowserHome = 0xAC,

	/// <summary>
	/// The volume mute key.
	/// </summary>
	VolumeMute = 0xAD,

	/// <summary>
	/// The volume down key.
	/// </summary>
	VolumeDown = 0xAE,

	/// <summary>
	/// The volume up key.
	/// </summary>
	VolumeUp = 0xAF,

	/// <summary>
	/// The media next track key.
	/// </summary>
	MediaNextTrack = 0xB0,

	/// <summary>
	/// The media previous track key.
	/// </summary>
	MediaPreviousTrack = 0xB1,

	/// <summary>
	/// The media stop key.
	/// </summary>
	MediaStop = 0xB2,

	/// <summary>
	/// The media play pause key.
	/// </summary>
	MediaPlayPause = 0xB3,

	/// <summary>
	/// The launch mail key.
	/// </summary>
	LaunchMail = 0xB4,

	/// <summary>
	/// The select media key.
	/// </summary>
	SelectMedia = 0xB5,

	/// <summary>
	/// The Launch Application1 key.
	/// </summary>
	LaunchApplication1 = 0xB6,

	/// <summary>
	/// The Launch Application2 key.
	/// </summary>
	LaunchApplication2 = 0xB7,

	/// <summary>
	/// The OEM Semicolon key.
	/// </summary>
	[Display(Name = "; (semicolon)")]
	OemSemicolon = 0xBA,

	/// <summary>
	/// The OEM 1 key.
	/// </summary>
	[Display(Name = "+ (plus)")]
	Oem1 = OemSemicolon,

	/// <summary>
	/// The OEM plus key.
	/// </summary>
	Oemplus = 0xBB,

	/// <summary>
	/// The OEM comma key.
	/// </summary>
	[Display(Name = ", (comma)")]
	Oemcomma = 0xBC,

	/// <summary>
	/// The OEM Minus key.
	/// </summary>
	[Display(Name = "- (minus)")]
	OemMinus = 0xBD,

	/// <summary>
	/// The OEM Period key.
	/// </summary>
	[Display(Name = ". (period)")]
	OemPeriod = 0xBE,

	/// <summary>
	/// The OEM question key.
	/// </summary>
	[Display(Name = "? (question mark)")]
	OemQuestion = 0xBF,

	/// <summary>
	/// The OEM 2 key.
	/// </summary>
	Oem2 = OemQuestion,

	/// <summary>
	/// The OEM 3 key.
	/// </summary>
	Oem3 = 0xC0,

	/// <summary>
	/// The OEM tilde key.
	/// </summary>
	[Display(Name = "~ (tilde)")]
	Oemtilde = Oem3,

	/// <summary>
	/// The OEM open brackets key.
	/// </summary>
	[Display(Name = "[ (open bracket)")]
	OemOpenBrackets = 0xDB,

	/// <summary>
	/// The OEM 4 key.
	/// </summary>
	Oem4 = OemOpenBrackets,

	/// <summary>
	/// The OEM Pipe key.
	/// </summary>
	[Display(Name = "| (pipe)")]
	OemPipe = 0xDC,

	/// <summary>
	/// The OEM 5 key.
	/// </summary>
	Oem5 = OemPipe,

	/// <summary>
	/// The OEM close brackets key.
	/// </summary>
	[Display(Name = "] (close bracket)")]
	OemCloseBrackets = 0xDD,

	/// <summary>
	/// The OEM 6 key.
	/// </summary>
	Oem6 = OemCloseBrackets,

	/// <summary>
	/// The OEM 7 key.
	/// </summary>
	Oem7 = 0xDE,

	/// <summary>
	/// The OEM Quotes key.
	/// </summary>
	[Display(Name = "\" (double quote)")]
	OemQuotes = Oem7,

	/// <summary>
	/// The OEM8 key.
	/// </summary>
	Oem8 = 0xDF,

	/// <summary>
	/// The OEM 102 key.
	/// </summary>
	Oem102 = 0xE2,

	/// <summary>
	/// The OEM Backslash key.
	/// </summary>
	[Display(Name = "\\ (backslash)")]
	OemBackslash = Oem102,

	/// <summary>
	/// The PROCESS KEY key.
	/// </summary>
	ProcessKey = 0xE5,

	/// <summary>
	/// The Packet KEY key.
	/// </summary>
	Packet = 0xE7,

	/// <summary>
	/// The ATTN key.
	/// </summary>
	Attn = 0xF6,

	/// <summary>
	/// The CRSEL key.
	/// </summary>
	Crsel = 0xF7,

	/// <summary>
	/// The EXSEL key.
	/// </summary>
	Exsel = 0xF8,

	/// <summary>
	/// The ERASE EOF key.
	/// </summary>
	EraseEof = 0xF9,

	/// <summary>
	/// The PLAY key.
	/// </summary>
	Play = 0xFA,

	/// <summary>
	/// The ZOOM key.
	/// </summary>
	Zoom = 0xFB,

	/// <summary>
	/// A constant reserved for future use.
	/// </summary>
	NoName = 0xFC,

	/// <summary>
	/// The PA1 key.
	/// </summary>
	Pa1 = 0xFD,

	/// <summary>
	/// The CLEAR key.
	/// </summary>
	OemClear = 0xFE,

	/// <summary>
	/// The SHIFT modifier key.
	/// </summary>
	Shift = 0x00010000,

	/// <summary>
	/// The CTRL modifier key.
	/// </summary>
	Control = 0x00020000,

	/// <summary>
	/// The ALT modifier key.
	/// </summary>
	Alt = 0x00040000,

	Super,
}