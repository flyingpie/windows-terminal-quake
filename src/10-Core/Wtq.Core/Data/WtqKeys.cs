namespace Wtq.Core.Data;

public enum WtqKeys
{
	//
	// Summary:
	//     The bitmask to extract a key code from a key value.
	KeyCode = 0xFFFF,

	//
	// Summary:
	//     The bitmask to extract modifiers from a key value.
	Modifiers = -65536,

	//
	// Summary:
	//     No key pressed.
	None = 0,

	//
	// Summary:
	//     The left mouse button.
	LButton = 1,

	//
	// Summary:
	//     The right mouse button.
	RButton = 2,

	//
	// Summary:
	//     The CANCEL key.
	Cancel = 3,

	//
	// Summary:
	//     The middle mouse button (three-button mouse).
	MButton = 4,

	//
	// Summary:
	//     The first x mouse button (five-button mouse).
	XButton1 = 5,

	//
	// Summary:
	//     The second x mouse button (five-button mouse).
	XButton2 = 6,

	//
	// Summary:
	//     The BACKSPACE key.
	Back = 8,

	//
	// Summary:
	//     The TAB key.
	Tab = 9,

	//
	// Summary:
	//     The LINEFEED key.
	LineFeed = 0xA,

	//
	// Summary:
	//     The CLEAR key.
	Clear = 0xC,

	//
	// Summary:
	//     The RETURN key.
	Return = 0xD,

	//
	// Summary:
	//     The ENTER key.
	Enter = 0xD,

	//
	// Summary:
	//     The SHIFT key.
	ShiftKey = 0x10,

	//
	// Summary:
	//     The CTRL key.
	ControlKey = 0x11,

	//
	// Summary:
	//     The ALT key.
	Menu = 0x12,

	//
	// Summary:
	//     The PAUSE key.
	Pause = 0x13,

	//
	// Summary:
	//     The CAPS LOCK key.
	Capital = 0x14,

	//
	// Summary:
	//     The CAPS LOCK key.
	CapsLock = 0x14,

	//
	// Summary:
	//     The IME Kana mode key.
	KanaMode = 0x15,

	//
	// Summary:
	//     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
	HanguelMode = 0x15,

	//
	// Summary:
	//     The IME Hangul mode key.
	HangulMode = 0x15,

	//
	// Summary:
	//     The IME Junja mode key.
	JunjaMode = 0x17,

	//
	// Summary:
	//     The IME final mode key.
	FinalMode = 0x18,

	//
	// Summary:
	//     The IME Hanja mode key.
	HanjaMode = 0x19,

	//
	// Summary:
	//     The IME Kanji mode key.
	KanjiMode = 0x19,

	//
	// Summary:
	//     The ESC key.
	Escape = 0x1B,

	//
	// Summary:
	//     The IME convert key.
	IMEConvert = 0x1C,

	//
	// Summary:
	//     The IME nonconvert key.
	IMENonconvert = 0x1D,

	//
	// Summary:
	//     The IME accept key, replaces System.Windows.Forms.Keys.IMEAceept.
	IMEAccept = 0x1E,

	//
	// Summary:
	//     The IME accept key. Obsolete, use System.Windows.Forms.Keys.IMEAccept instead.
	IMEAceept = 0x1E,

	//
	// Summary:
	//     The IME mode change key.
	IMEModeChange = 0x1F,

	//
	// Summary:
	//     The SPACEBAR key.
	Space = 0x20,

	//
	// Summary:
	//     The PAGE UP key.
	Prior = 0x21,

	//
	// Summary:
	//     The PAGE UP key.
	PageUp = 0x21,

	//
	// Summary:
	//     The PAGE DOWN key.
	Next = 0x22,

	//
	// Summary:
	//     The PAGE DOWN key.
	PageDown = 0x22,

	//
	// Summary:
	//     The END key.
	End = 0x23,

	//
	// Summary:
	//     The HOME key.
	Home = 0x24,

	//
	// Summary:
	//     The LEFT ARROW key.
	Left = 0x25,

	//
	// Summary:
	//     The UP ARROW key.
	Up = 0x26,

	//
	// Summary:
	//     The RIGHT ARROW key.
	Right = 0x27,

	//
	// Summary:
	//     The DOWN ARROW key.
	Down = 0x28,

	//
	// Summary:
	//     The SELECT key.
	Select = 0x29,

	//
	// Summary:
	//     The PRINT key.
	Print = 0x2A,

	//
	// Summary:
	//     The EXECUTE key.
	Execute = 0x2B,

	//
	// Summary:
	//     The PRINT SCREEN key.
	Snapshot = 0x2C,

	//
	// Summary:
	//     The PRINT SCREEN key.
	PrintScreen = 0x2C,

	//
	// Summary:
	//     The INS key.
	Insert = 0x2D,

	//
	// Summary:
	//     The DEL key.
	Delete = 0x2E,

	//
	// Summary:
	//     The HELP key.
	Help = 0x2F,

	//
	// Summary:
	//     The 0 key.
	D0 = 0x30,

	//
	// Summary:
	//     The 1 key.
	D1 = 0x31,

	//
	// Summary:
	//     The 2 key.
	D2 = 0x32,

	//
	// Summary:
	//     The 3 key.
	D3 = 0x33,

	//
	// Summary:
	//     The 4 key.
	D4 = 0x34,

	//
	// Summary:
	//     The 5 key.
	D5 = 0x35,

	//
	// Summary:
	//     The 6 key.
	D6 = 0x36,

	//
	// Summary:
	//     The 7 key.
	D7 = 0x37,

	//
	// Summary:
	//     The 8 key.
	D8 = 0x38,

	//
	// Summary:
	//     The 9 key.
	D9 = 0x39,

	//
	// Summary:
	//     The A key.
	A = 0x41,

	//
	// Summary:
	//     The B key.
	B = 0x42,

	//
	// Summary:
	//     The C key.
	C = 0x43,

	//
	// Summary:
	//     The D key.
	D = 0x44,

	//
	// Summary:
	//     The E key.
	E = 0x45,

	//
	// Summary:
	//     The F key.
	F = 0x46,

	//
	// Summary:
	//     The G key.
	G = 0x47,

	//
	// Summary:
	//     The H key.
	H = 0x48,

	//
	// Summary:
	//     The I key.
	I = 0x49,

	//
	// Summary:
	//     The J key.
	J = 0x4A,

	//
	// Summary:
	//     The K key.
	K = 0x4B,

	//
	// Summary:
	//     The L key.
	L = 0x4C,

	//
	// Summary:
	//     The M key.
	M = 0x4D,

	//
	// Summary:
	//     The N key.
	N = 0x4E,

	//
	// Summary:
	//     The O key.
	O = 0x4F,

	//
	// Summary:
	//     The P key.
	P = 0x50,

	//
	// Summary:
	//     The Q key.
	Q = 0x51,

	//
	// Summary:
	//     The R key.
	R = 0x52,

	//
	// Summary:
	//     The S key.
	S = 0x53,

	//
	// Summary:
	//     The T key.
	T = 0x54,

	//
	// Summary:
	//     The U key.
	U = 0x55,

	//
	// Summary:
	//     The V key.
	V = 0x56,

	//
	// Summary:
	//     The W key.
	W = 0x57,

	//
	// Summary:
	//     The X key.
	X = 0x58,

	//
	// Summary:
	//     The Y key.
	Y = 0x59,

	//
	// Summary:
	//     The Z key.
	Z = 0x5A,

	//
	// Summary:
	//     The left Windows logo key (Microsoft Natural Keyboard).
	LWin = 0x5B,

	//
	// Summary:
	//     The right Windows logo key (Microsoft Natural Keyboard).
	RWin = 0x5C,

	//
	// Summary:
	//     The application key (Microsoft Natural Keyboard).
	Apps = 0x5D,

	//
	// Summary:
	//     The computer sleep key.
	Sleep = 0x5F,

	//
	// Summary:
	//     The 0 key on the numeric keypad.
	NumPad0 = 0x60,

	//
	// Summary:
	//     The 1 key on the numeric keypad.
	NumPad1 = 0x61,

	//
	// Summary:
	//     The 2 key on the numeric keypad.
	NumPad2 = 0x62,

	//
	// Summary:
	//     The 3 key on the numeric keypad.
	NumPad3 = 0x63,

	//
	// Summary:
	//     The 4 key on the numeric keypad.
	NumPad4 = 0x64,

	//
	// Summary:
	//     The 5 key on the numeric keypad.
	NumPad5 = 0x65,

	//
	// Summary:
	//     The 6 key on the numeric keypad.
	NumPad6 = 0x66,

	//
	// Summary:
	//     The 7 key on the numeric keypad.
	NumPad7 = 0x67,

	//
	// Summary:
	//     The 8 key on the numeric keypad.
	NumPad8 = 0x68,

	//
	// Summary:
	//     The 9 key on the numeric keypad.
	NumPad9 = 0x69,

	//
	// Summary:
	//     The multiply key.
	Multiply = 0x6A,

	//
	// Summary:
	//     The add key.
	Add = 0x6B,

	//
	// Summary:
	//     The separator key.
	Separator = 0x6C,

	//
	// Summary:
	//     The subtract key.
	Subtract = 0x6D,

	//
	// Summary:
	//     The decimal key.
	Decimal = 0x6E,

	//
	// Summary:
	//     The divide key.
	Divide = 0x6F,

	//
	// Summary:
	//     The F1 key.
	F1 = 0x70,

	//
	// Summary:
	//     The F2 key.
	F2 = 0x71,

	//
	// Summary:
	//     The F3 key.
	F3 = 0x72,

	//
	// Summary:
	//     The F4 key.
	F4 = 0x73,

	//
	// Summary:
	//     The F5 key.
	F5 = 0x74,

	//
	// Summary:
	//     The F6 key.
	F6 = 0x75,

	//
	// Summary:
	//     The F7 key.
	F7 = 0x76,

	//
	// Summary:
	//     The F8 key.
	F8 = 0x77,

	//
	// Summary:
	//     The F9 key.
	F9 = 0x78,

	//
	// Summary:
	//     The F10 key.
	F10 = 0x79,

	//
	// Summary:
	//     The F11 key.
	F11 = 0x7A,

	//
	// Summary:
	//     The F12 key.
	F12 = 0x7B,

	//
	// Summary:
	//     The F13 key.
	F13 = 0x7C,

	//
	// Summary:
	//     The F14 key.
	F14 = 0x7D,

	//
	// Summary:
	//     The F15 key.
	F15 = 0x7E,

	//
	// Summary:
	//     The F16 key.
	F16 = 0x7F,

	//
	// Summary:
	//     The F17 key.
	F17 = 0x80,

	//
	// Summary:
	//     The F18 key.
	F18 = 0x81,

	//
	// Summary:
	//     The F19 key.
	F19 = 0x82,

	//
	// Summary:
	//     The F20 key.
	F20 = 0x83,

	//
	// Summary:
	//     The F21 key.
	F21 = 0x84,

	//
	// Summary:
	//     The F22 key.
	F22 = 0x85,

	//
	// Summary:
	//     The F23 key.
	F23 = 0x86,

	//
	// Summary:
	//     The F24 key.
	F24 = 0x87,

	//
	// Summary:
	//     The NUM LOCK key.
	NumLock = 0x90,

	//
	// Summary:
	//     The SCROLL LOCK key.
	Scroll = 0x91,

	//
	// Summary:
	//     The left SHIFT key.
	LShiftKey = 0xA0,

	//
	// Summary:
	//     The right SHIFT key.
	RShiftKey = 0xA1,

	//
	// Summary:
	//     The left CTRL key.
	LControlKey = 0xA2,

	//
	// Summary:
	//     The right CTRL key.
	RControlKey = 0xA3,

	//
	// Summary:
	//     The left ALT key.
	LMenu = 0xA4,

	//
	// Summary:
	//     The right ALT key.
	RMenu = 0xA5,

	//
	// Summary:
	//     The browser back key (Windows 2000 or later).
	BrowserBack = 0xA6,

	//
	// Summary:
	//     The browser forward key (Windows 2000 or later).
	BrowserForward = 0xA7,

	//
	// Summary:
	//     The browser refresh key (Windows 2000 or later).
	BrowserRefresh = 0xA8,

	//
	// Summary:
	//     The browser stop key (Windows 2000 or later).
	BrowserStop = 0xA9,

	//
	// Summary:
	//     The browser search key (Windows 2000 or later).
	BrowserSearch = 0xAA,

	//
	// Summary:
	//     The browser favorites key (Windows 2000 or later).
	BrowserFavorites = 0xAB,

	//
	// Summary:
	//     The browser home key (Windows 2000 or later).
	BrowserHome = 0xAC,

	//
	// Summary:
	//     The volume mute key (Windows 2000 or later).
	VolumeMute = 0xAD,

	//
	// Summary:
	//     The volume down key (Windows 2000 or later).
	VolumeDown = 0xAE,

	//
	// Summary:
	//     The volume up key (Windows 2000 or later).
	VolumeUp = 0xAF,

	//
	// Summary:
	//     The media next track key (Windows 2000 or later).
	MediaNextTrack = 0xB0,

	//
	// Summary:
	//     The media previous track key (Windows 2000 or later).
	MediaPreviousTrack = 0xB1,

	//
	// Summary:
	//     The media Stop key (Windows 2000 or later).
	MediaStop = 0xB2,

	//
	// Summary:
	//     The media play pause key (Windows 2000 or later).
	MediaPlayPause = 0xB3,

	//
	// Summary:
	//     The launch mail key (Windows 2000 or later).
	LaunchMail = 0xB4,

	//
	// Summary:
	//     The select media key (Windows 2000 or later).
	SelectMedia = 0xB5,

	//
	// Summary:
	//     The start application one key (Windows 2000 or later).
	LaunchApplication1 = 0xB6,

	//
	// Summary:
	//     The start application two key (Windows 2000 or later).
	LaunchApplication2 = 0xB7,

	//
	// Summary:
	//     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
	OemSemicolon = 0xBA,

	//
	// Summary:
	//     The OEM 1 key.
	Oem1 = 0xBA,

	//
	// Summary:
	//     The OEM plus key on any country/region keyboard (Windows 2000 or later).
	Oemplus = 0xBB,

	//
	// Summary:
	//     The OEM comma key on any country/region keyboard (Windows 2000 or later).
	Oemcomma = 0xBC,

	//
	// Summary:
	//     The OEM minus key on any country/region keyboard (Windows 2000 or later).
	OemMinus = 0xBD,

	//
	// Summary:
	//     The OEM period key on any country/region keyboard (Windows 2000 or later).
	OemPeriod = 0xBE,

	//
	// Summary:
	//     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
	OemQuestion = 0xBF,

	//
	// Summary:
	//     The OEM 2 key.
	Oem2 = 0xBF,

	//
	// Summary:
	//     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
	Oemtilde = 0xC0,

	//
	// Summary:
	//     The OEM 3 key.
	Oem3 = 0xC0,

	//
	// Summary:
	//     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
	OemOpenBrackets = 0xDB,

	//
	// Summary:
	//     The OEM 4 key.
	Oem4 = 0xDB,

	//
	// Summary:
	//     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
	OemPipe = 0xDC,

	//
	// Summary:
	//     The OEM 5 key.
	Oem5 = 0xDC,

	//
	// Summary:
	//     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
	OemCloseBrackets = 0xDD,

	//
	// Summary:
	//     The OEM 6 key.
	Oem6 = 0xDD,

	//
	// Summary:
	//     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
	OemQuotes = 0xDE,

	//
	// Summary:
	//     The OEM 7 key.
	Oem7 = 0xDE,

	//
	// Summary:
	//     The OEM 8 key.
	Oem8 = 0xDF,

	//
	// Summary:
	//     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
	//     or later).
	OemBackslash = 0xE2,

	//
	// Summary:
	//     The OEM 102 key.
	Oem102 = 0xE2,

	//
	// Summary:
	//     The PROCESS KEY key.
	ProcessKey = 0xE5,

	//
	// Summary:
	//     Used to pass Unicode characters as if they were keystrokes. The Packet key value
	//     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
	Packet = 0xE7,

	//
	// Summary:
	//     The ATTN key.
	Attn = 0xF6,

	//
	// Summary:
	//     The CRSEL key.
	Crsel = 0xF7,

	//
	// Summary:
	//     The EXSEL key.
	Exsel = 0xF8,

	//
	// Summary:
	//     The ERASE EOF key.
	EraseEof = 0xF9,

	//
	// Summary:
	//     The PLAY key.
	Play = 0xFA,

	//
	// Summary:
	//     The ZOOM key.
	Zoom = 0xFB,

	//
	// Summary:
	//     A constant reserved for future use.
	NoName = 0xFC,

	//
	// Summary:
	//     The PA1 key.
	Pa1 = 0xFD,

	//
	// Summary:
	//     The CLEAR key.
	OemClear = 0xFE,

	//
	// Summary:
	//     The SHIFT modifier key.
	Shift = 0x10000,

	//
	// Summary:
	//     The CTRL modifier key.
	Control = 0x20000,

	//
	// Summary:
	//     The ALT modifier key.
	Alt = 0x40000,
}