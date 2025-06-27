namespace Wtq.Input;

/// <summary>
/// Represents a virtual key code, i.e. a key that points to a physical button on a keyboard, without necessarily taking into account keyboard language/layout.
/// </summary>
[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "MvdO: Some overlap for convenience.")]
[SuppressMessage("Info Code Smell", "S1133:Deprecated code should be removed", Justification = "MvdO: For backward compat.")]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "MvdO: Very useful for large enum.")]
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "MvdO: Trying to stay consistent with naming on other platforms, libraries and frameworks.")]
public enum KeyCode
{
	/// <summary>No key pressed.</summary>
	None = 0x00,

	#region The Big Keys

	/// <summary>The BACKSPACE key.</summary>
	Backspace = 0x08,

	/// <summary>The CONTEXT MENU key, usually located to the right of the space bar (if present at all)..</summary>
	ContextMenu = 0x5D,

	/// <summary>The ESC key.</summary>
	Escape = 0x1B,

	/// <summary>The PAUSE key.</summary>
	Pause = 0x13,

	/// <summary>The PRINT SCREEN key.</summary>
	PrintScreen = 0x2C,

	/// <summary>The RETURN/ENTER key.</summary>
	Return = 0x0D,

	/// <summary>The SPACE BAR key.</summary>
	Space = 0x20,

	/// <summary>The TAB key.</summary>
	Tab = 0x09,

	#endregion

	#region Modifiers

	/// <summary>The left ALT key.</summary>
	AltLeft = 0xA4,

	/// <summary>The right ALT key.</summary>
	AltRight = 0xA5,

	/// <summary>The left CTRL key.</summary>
	ControlLeft = 0xA2,

	/// <summary>The right CTRL key.</summary>
	ControlRight = 0xA3,

	/// <summary>The left SHIFT key.</summary>
	ShiftLeft = 0xA0,

	/// <summary>The right SHIFT key.</summary>
	ShiftRight = 0xA1,

	/// <summary>The left "Windows" key (also known as "Meta" or "Super").</summary>
	SuperLeft = 0x5B,

	/// <summary>The right "Windows" key (also known as "Meta" or "Super").</summary>
	SuperRight = 0x5C,

	#endregion

	#region Lock keys

	/// <summary>The CAPS LOCK key.</summary>
	CapsLock = 0x14,

	/// <summary>The NUM LOCK key.</summary>
	NumLock = 0x90,

	/// <summary>The SCROLL LOCK key.</summary>
	ScrollLock = 0x91,

	#endregion

	#region F-Keys

	/// <summary>The "F1" key.</summary>
	F1 = 0x70,

	/// <summary>The "F2" key.</summary>
	F2 = 0x71,

	/// <summary>The "F3" key.</summary>
	F3 = 0x72,

	/// <summary>The "F4" key.</summary>
	F4 = 0x73,

	/// <summary>The "F5" key.</summary>
	F5 = 0x74,

	/// <summary>The "F6" key.</summary>
	F6 = 0x75,

	/// <summary>The "F7" key.</summary>
	F7 = 0x76,

	/// <summary>The "F8" key.</summary>
	F8 = 0x77,

	/// <summary>The "F9" key.</summary>
	F9 = 0x78,

	/// <summary>The "F10" key.</summary>
	F10 = 0x79,

	/// <summary>The "F11" key.</summary>
	F11 = 0x7A,

	/// <summary>The "F12" key.</summary>
	F12 = 0x7B,

	/// <summary>The "F13" key.</summary>
	F13 = 0x7C,

	/// <summary>The "F14" key.</summary>
	F14 = 0x7D,

	/// <summary>The "F15" key.</summary>
	F15 = 0x7E,

	/// <summary>The "F16" key.</summary>
	F16 = 0x7F,

	/// <summary>The "F17" key.</summary>
	F17 = 0x80,

	/// <summary>The "F18" key.</summary>
	F18 = 0x81,

	/// <summary>The "F19" key.</summary>
	F19 = 0x82,

	/// <summary>The "F20" key.</summary>
	F20 = 0x83,

	/// <summary>The "F21" key.</summary>
	F21 = 0x84,

	/// <summary>The "F22" key.</summary>
	F22 = 0x85,

	/// <summary>The "F23" key.</summary>
	F23 = 0x86,

	/// <summary>The "F24" key.</summary>
	F24 = 0x87,

	#endregion

	#region Main Row - Numbers

	/// <summary>Main row "0".</summary>
	D0 = 0x30,

	/// <summary>Main row "1".</summary>
	D1 = 0x31,

	/// <summary>Main row "2".</summary>
	D2 = 0x32,

	/// <summary>Main row "3".</summary>
	D3 = 0x33,

	/// <summary>Main row "4".</summary>
	D4 = 0x34,

	/// <summary>Main row "5".</summary>
	D5 = 0x35,

	/// <summary>Main row "6".</summary>
	D6 = 0x36,

	/// <summary>Main row "7".</summary>
	D7 = 0x37,

	/// <summary>Main row "8".</summary>
	D8 = 0x38,

	/// <summary>Main row "9".</summary>
	D9 = 0x39,

	#endregion

	#region Main Row - Letters

	/// <summary>Main row "A".</summary>
	A = 0x41,

	/// <summary>Main row "B".</summary>
	B = 0x42,

	/// <summary>Main row "C".</summary>
	C = 0x43,

	/// <summary>Main row "D".</summary>
	D = 0x44,

	/// <summary>Main row "E".</summary>
	E = 0x45,

	/// <summary>Main row "F".</summary>
	F = 0x46,

	/// <summary>Main row "G".</summary>
	G = 0x47,

	/// <summary>Main row "H".</summary>
	H = 0x48,

	/// <summary>Main row "I".</summary>
	I = 0x49,

	/// <summary>Main row "J".</summary>
	J = 0x4A,

	/// <summary>Main row "K".</summary>
	K = 0x4B,

	/// <summary>Main row "L".</summary>
	L = 0x4C,

	/// <summary>Main row "M".</summary>
	M = 0x4D,

	/// <summary>Main row "N".</summary>
	N = 0x4E,

	/// <summary>Main row "O".</summary>
	O = 0x4F,

	/// <summary>Main row "P".</summary>
	P = 0x50,

	/// <summary>Main row "Q".</summary>
	Q = 0x51,

	/// <summary>Main row "R".</summary>
	R = 0x52,

	/// <summary>Main row "S".</summary>
	S = 0x53,

	/// <summary>Main row "T".</summary>
	T = 0x54,

	/// <summary>Main row "U".</summary>
	U = 0x55,

	/// <summary>Main row "V".</summary>
	V = 0x56,

	/// <summary>Main row "W".</summary>
	W = 0x57,

	/// <summary>Main row "X".</summary>
	X = 0x58,

	/// <summary>Main row "Y".</summary>
	Y = 0x59,

	/// <summary>Main row "Z".</summary>
	Z = 0x5A,

	#endregion

	#region OEM Keys

	/// <summary>The OEM 1 key.</summary>
	Oem1 = 0xBA,

	/// <summary>The OEM 2 key.</summary>
	Oem2 = 0xBF,

	/// <summary>The OEM 3 key.</summary>
	Oem3 = 0xC0,

	/// <summary>The OEM 4 key.</summary>
	Oem4 = 0xDB,

	/// <summary>The OEM 5 key.</summary>
	Oem5 = 0xDC,

	/// <summary>The OEM 6 key.</summary>
	Oem6 = 0xDD,

	/// <summary>The OEM 7 key.</summary>
	Oem7 = 0xDE,

	/// <summary>The OEM 8 key.</summary>
	Oem8 = 0xDF,

	/// <summary>The OEM 102 key.</summary>
	Oem102 = 0xE2,

	/// <summary>The CLEAR key.</summary>
	OemClear = 0xFE,

	/// <summary>The OEM "," key.</summary>
	OemComma = 0xBC,

	/// <summary>The OEM "-" key.</summary>
	OemMinus = 0xBD,

	/// <summary>The OEM "." key.</summary>
	OemPeriod = 0xBE,

	/// <summary>The OEM "+" key.</summary>
	OemPlus = 0xBB,

	#endregion

	#region OEM Aliases (US Layouts)

	/// <summary>The OEM ";" key.</summary>
	OemSemicolon = Oem1,

	/// <summary>The OEM "?" key.</summary>
	OemQuestion = Oem2,

	/// <summary>The OEM "~" key.</summary>
	OemTilde = Oem3,

	/// <summary>The OEM "[" key.</summary>
	OemOpenBrackets = Oem4,

	/// <summary>The OEM "|" key.</summary>
	OemPipe = Oem5,

	/// <summary>The OEM "]" key.</summary>
	OemCloseBrackets = Oem6,

	/// <summary>The OEM " key.</summary>
	OemQuotes = Oem7,

	/// <summary>The OEM "\" key.</summary>
	OemBackslash = Oem102,

	#endregion

	#region Numpad (or "Keypad")

	/// <summary>Numpad "0" key.</summary>
	Numpad0 = 0x60,

	/// <summary>Numpad "1" key.</summary>
	Numpad1 = 0x61,

	/// <summary>Numpad "2" key.</summary>
	Numpad2 = 0x62,

	/// <summary>Numpad "3" key.</summary>
	Numpad3 = 0x63,

	/// <summary>Numpad "4" key.</summary>
	Numpad4 = 0x64,

	/// <summary>Numpad "5" key.</summary>
	Numpad5 = 0x65,

	/// <summary>Numpad "6" key.</summary>
	Numpad6 = 0x66,

	/// <summary>Numpad "7" key.</summary>
	Numpad7 = 0x67,

	/// <summary>Numpad "8" key.</summary>
	Numpad8 = 0x68,

	/// <summary>Numpad "9" key.</summary>
	Numpad9 = 0x69,

	/// <summary>Numpad "*" key.</summary>
	NumpadMultiply = 0x6A,

	/// <summary>Numpad "+" key.</summary>
	NumpadAdd = 0x6B,

	/// <summary>Numpad separator key (not available on US keyboards).</summary>
	NumpadSeparator = 0x6C,

	/// <summary>Numpad "-" key.</summary>
	NumpadSubtract = 0x6D,

	/// <summary>Numpad "." key (or whatever is used as decimal separator for the particular region).</summary>
	NumpadDecimal = 0x6E,

	/// <summary>Numpad "/" key.</summary>
	NumpadDivide = 0x6F,

	#endregion

	#region Above Arrow Keys

	/// <summary>The INS key.</summary>
	Insert = 0x2D,

	/// <summary>The DEL key.</summary>
	Delete = 0x2E,

	/// <summary>The HOME key.</summary>
	Home = 0x24,

	/// <summary>The END key.</summary>
	End = 0x23,

	/// <summary>The PAGE UP key.</summary>
	PageUp = 0x21,

	/// <summary>The PAGE DOWN key.</summary>
	PageDown = 0x22,

	#endregion

	#region Arrow Keys

	/// <summary>The UP ARROW key.</summary>
	ArrowUp = 0x26,

	/// <summary>The DOWN ARROW key.</summary>
	ArrowDown = 0x28,

	/// <summary>The LEFT ARROW key.</summary>
	ArrowLeft = 0x25,

	/// <summary>The RIGHT ARROW key.</summary>
	ArrowRight = 0x27,

	#endregion

	#region Browser Keys

	/// <summary>The browser back key.</summary>
	BrowserBack = 0xA6,

	/// <summary>The browser forward key.</summary>
	BrowserForward = 0xA7,

	/// <summary>The browser refresh key.</summary>
	BrowserRefresh = 0xA8,

	/// <summary>The browser stop key.</summary>
	BrowserStop = 0xA9,

	/// <summary>The browser search key.</summary>
	BrowserSearch = 0xAA,

	/// <summary>The browser favorites key.</summary>
	BrowserFavorites = 0xAB,

	/// <summary>The browser home key.</summary>
	BrowserHome = 0xAC,

	#endregion

	#region Volume Keys

	/// <summary>The VOLUME MUTE key.</summary>
	VolumeMute = 0xAD,

	/// <summary>The VOLUME DOWN key.</summary>
	VolumeDown = 0xAE,

	/// <summary>The VOLUME UP key.</summary>
	VolumeUp = 0xAF,

	#endregion

	#region Media Keys

	/// <summary>The MEDIA NEXT TRACK key.</summary>
	MediaNextTrack = 0xB0,

	/// <summary>The MEDIA PREVIOUS TRACK key.</summary>
	MediaPreviousTrack = 0xB1,

	/// <summary>The MEDIA STOP key.</summary>
	MediaStop = 0xB2,

	/// <summary>The MEDIA PLAY/PAUSE key.</summary>
	MediaPlayPause = 0xB3,

	#endregion

	#region IME Keys

	/// <summary>The IME Convert key.</summary>
	IMEConvert = 0x1C,

	/// <summary>The IME NonConvert key.</summary>
	IMENonConvert = 0x1D,

	/// <summary>The IME Accept key.</summary>
	IMEAccept = 0x1E,

	/// <summary>The IME Mode change request.</summary>
	IMEModeChange = 0x1F,

	/// <summary>The IME Kana mode key.</summary>
	KanaMode = 0x15,

	/// <summary>The IME Hanguel mode key.</summary>
	HanguelMode = 0x15,

	/// <summary>The IME Hangul mode key.</summary>
	HangulMode = 0x15,

	/// <summary>The IME Junja mode key.</summary>
	JunjaMode = 0x17,

	/// <summary>The IME Final mode key.</summary>
	FinalMode = 0x18,

	/// <summary>The IME Hanja mode key.</summary>
	HanjaMode = 0x19,

	/// <summary>The IME Kanji mode key.</summary>
	KanjiMode = 0x19,

	#endregion

	#region Launch Keys

	/// <summary>The launch mail key.</summary>
	LaunchMail = 0xB4,

	/// <summary>The launch select media key.</summary>
	LaunchSelectMedia = 0xB5,

	/// <summary>The Launch APP1 key.</summary>
	LaunchApplication1 = 0xB6,

	/// <summary>The Launch APP2 key.</summary>
	LaunchApplication2 = 0xB7,

	#endregion

	#region Misc

	/// <summary>The ATTN key.</summary>
	Attn = 0xF6,

	/// <summary>The CANCEL key.</summary>
	Cancel = 0x03,

	/// <summary>The CLEAR key.</summary>
	Clear = 0x0C,

	/// <summary>The CRSEL key.</summary>
	Crsel = 0xF7,

	/// <summary>The ERASE EOF key.</summary>
	EraseEof = 0xF9,

	/// <summary>The EXECUTE key.</summary>
	Execute = 0x2B,

	/// <summary>The EXSEL key.</summary>
	Exsel = 0xF8,

	/// <summary>The HELP key.</summary>
	Help = 0x2F,

	/// <summary>The LINEFEED key.</summary>
	LineFeed = 0x0A,

	/// <summary>A constant reserved for future use.</summary>
	NoName = 0xFC,

	/// <summary>The PA1 key.</summary>
	Pa1 = 0xFD,

	/// <summary>The PACKET key.</summary>
	Packet = 0xE7,

	/// <summary>The PLAY key.</summary>
	Play = 0xFA,

	/// <summary>The PRINT key.</summary>
	Print = 0x2A,

	/// <summary>The PROCESS key.</summary>
	Process = 0xE5,

	/// <summary>The SELECT key.</summary>
	Select = 0x29,

	/// <summary>The computer sleep key.</summary>
	Sleep = 0x5F,

	/// <summary>The ZOOM key.</summary>
	Zoom = 0xFB,

	#endregion
}