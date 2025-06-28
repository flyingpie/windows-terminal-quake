#pragma warning disable

using Wtq.Input;
using KC = Wtq.Input.KeyCode;
using KM = Wtq.Input.KeyModifiers;

namespace Wtq.Core.UnitTest.Input;

// @formatter:off
[TestClass]
public class KeySequenceTest
{
	[TestMethod]
	[DataRow(	KM.None,						null,		KC.None,		false)]
	[DataRow(	KM.Alt,							null,		KC.None,		true)]
	[DataRow(	KM.Alt | KM.Control,			null,		KC.None,		true)]
	public void HasModifiersTest(KM mod, string? kChar, KC kCode, bool isEmpty)
	{
		Assert.AreEqual(isEmpty, new KeySequence(mod, kChar, kCode).HasModifiers);
	}

	[TestMethod]
	[DataRow(	KM.None,	null,		KC.None,		false)]
	[DataRow(	KM.None,	"A",		KC.None,		true)]
	public void HasKeyCharTest(KM mod, string? kChar, KC kCode, bool isEmpty)
	{
		Assert.AreEqual(isEmpty, new KeySequence(mod, kChar, kCode).HasKeyChar);
	}

	[TestMethod]
	[DataRow(	KM.None,	null,		KC.None,	false)]
	[DataRow(	KM.None,	null,		KC.A,		true)]
	public void HasKeyCodeTest(KM mod, string? kChar, KC kCode, bool isEmpty)
	{
		Assert.AreEqual(isEmpty, new KeySequence(mod, kChar, kCode).HasKeyCode);
	}

	[TestMethod]
	[DataRow(	KM.None,						null,		KC.None,		true)]
	//																		//
	[DataRow(	KM.Alt,							null,		KC.None,		false)]
	[DataRow(	KM.Alt | KM.Control,			null,		KC.None,		false)]
	//																		//
	[DataRow(	KM.None,						"A",		KC.None,		false)]
	[DataRow(	KM.None,						null,		KC.A,			false)]
	[DataRow(	KM.None,						"A",		KC.A,			false)]
	public void IsEmptyTest(KM mod, string? kChar, KC kCode, bool isEmpty)
	{
		Assert.AreEqual(isEmpty, new KeySequence(mod, kChar, kCode).IsEmpty);
	}

	[TestMethod]
	// 			ModifierA														KeyCharA		KeyCodeA			ModifierB														KeyCharB		KeyCodeB			AreEqual
	//																																																					//
	// Key char only																																																	//
	[DataRow(	KM.None,														"A",			KC.None,			KM.None,														"A",			KC.None,			true)]
	[DataRow(	KM.None,														"A",			KC.None,			KM.None,														"B",			KC.None,			false)]
	[DataRow(	KM.None,														"B",			KC.None,			KM.None,														"A",			KC.None,			false)]
	[DataRow(	KM.None,														"A",			KC.None,			KM.None,														"a",			KC.None,			true)]
	[DataRow(	KM.None,														"A",			KC.None,			KM.None,														"b",			KC.None,			false)]
	[DataRow(	KM.None,														"B",			KC.None,			KM.None,														"a",			KC.None,			false)]
	[DataRow(	KM.None,														"a",			KC.None,			KM.None,														"A",			KC.None,			true)]
	[DataRow(	KM.None,														"a",			KC.None,			KM.None,														"B",			KC.None,			false)]
	[DataRow(	KM.None,														"b",			KC.None,			KM.None,														"A",			KC.None,			false)]
	//																																																					//
	// Key code only																																																	//
	[DataRow(	KM.None,														null,			KC.A,				KM.None,														null,			KC.A,				true)]
	[DataRow(	KM.None,														null,			KC.A,				KM.None,														null,			KC.B,				false)]
	[DataRow(	KM.None,														null,			KC.B,				KM.None,														null,			KC.A,				false)]
	//																																																					//
	// Key char & key code																																																//
	[DataRow(	KM.None,														"A",			KC.A,				KM.None,														"A",			KC.A,				true)]
	[DataRow(	KM.None,														"A",			KC.A,				KM.None,														"A",			KC.A,				true)]
	[DataRow(	KM.None,														"B",			KC.B,				KM.None,														"B",			KC.A,				true)]
	//																																																					//
	[DataRow(	KM.None,														"B",			KC.B,				KM.None,														"A",			KC.A,				false)]
	[DataRow(	KM.None,														"B",			KC.B,				KM.None,														"A",			KC.A,				false)]
	[DataRow(	KM.None,														"A",			KC.A,				KM.None,														"B",			KC.B,				false)]
	//																																																					//
	// Modifiers + key char																																																//
	[DataRow(	KM.None,														"`",			KC.None,			KM.None,														"`",			KC.None,			true)]
	[DataRow(	KM.None,														"`",			KC.None,			KM.None,														"~",			KC.None,			false)]
	//																																																					//
	// Modifiers + key code																																																//
	[DataRow(	KM.None,														null,			KC.A,				KM.None,														null,			KC.A,				true)]
	//																																																					//
	[DataRow(	KM.Alt,															null,			KC.A,				KM.Alt,															null,			KC.A,				true)]
	[DataRow(	KM.Control,														null,			KC.A,				KM.Control,														null,			KC.A,				true)]
	[DataRow(	KM.Numpad,														null,			KC.A,				KM.Numpad,														null,			KC.A,				true)]
	[DataRow(	KM.Shift,														null,			KC.A,				KM.Shift,														null,			KC.A,				true)]
	[DataRow(	KM.Super,														null,			KC.A,				KM.Super,														null,			KC.A,				true)]
	//																																																					//
	[DataRow(	KM.Alt,															null,			KC.A,				KM.None,														null,			KC.A,				false)]
	[DataRow(	KM.Control,														null,			KC.A,				KM.None,														null,			KC.A,				false)]
	[DataRow(	KM.Numpad,														null,			KC.A,				KM.None,														null,			KC.A,				false)]
	[DataRow(	KM.Shift,														null,			KC.A,				KM.None,														null,			KC.A,				false)]
	[DataRow(	KM.Super,														null,			KC.A,				KM.None,														null,			KC.A,				false)]
	[DataRow(	KM.None,														null,			KC.A,				KM.Alt,															null,			KC.A,				false)]
	[DataRow(	KM.None,														null,			KC.A,				KM.Control,														null,			KC.A,				false)]
	[DataRow(	KM.None,														null,			KC.A,				KM.Numpad,														null,			KC.A,				false)]
	[DataRow(	KM.None,														null,			KC.A,				KM.Shift,														null,			KC.A,				false)]
	[DataRow(	KM.None,														null,			KC.A,				KM.Super,														null,			KC.A,				false)]
	//																																																					//
	[DataRow(	KM.Alt | KM.Control,											null,			KC.A,				KM.Alt | KM.Control,											null,			KC.A,				true)]
	[DataRow(	KM.Alt | KM.Control,											null,			KC.A,				KM.Control | KM.Shift,											null,			KC.A,				false)]
	//																																																					//
	[DataRow(	KM.Alt | KM.Control | KM.Numpad | KM.Shift | KM.Super,			"A",			KC.None,			KM.Alt | KM.Control | KM.Numpad | KM.Shift | KM.Super,			"A",			KC.None,			true)]
	[DataRow(	KM.Alt | KM.Control | KM.Numpad | KM.Shift | KM.Super,			null,			KC.A,				KM.Alt | KM.Control | KM.Numpad | KM.Shift | KM.Super,			null,			KC.A,				true)]
	public void EqualsTest(KM modA, string? kCharA, KC kCodeA, KM modB, string? kCharB, KC kCodeB, bool areEqual)
	{
		Assert.AreEqual(areEqual, new KeySequence(modA, kCharA, kCodeA) == new KeySequence(modB, kCharB, kCodeB));
	}

	[TestMethod]
	[DataRow(	KM.None,		null,			false)]
	//											//
	[DataRow(	KM.None,		"a",			false)]
	[DataRow(	KM.Shift,		"a",			false)]
	[DataRow(	KM.None,		"A",			false)]
	[DataRow(	KM.Shift,		"A",			false)]
	[DataRow(	KM.None,		"1",			false)]
	[DataRow(	KM.Shift,		"1",			true)]
	//											//
	[DataRow(	KM.None,		"!",			false)]
	[DataRow(	KM.Shift,		"!",			true)]
	[DataRow(	KM.None,		";",			false)]
	[DataRow(	KM.Shift,		";",			true)]
	public void IsShiftImpliedTest(KM mod, string? kChar, bool areEqual)
	{
		Assert.AreEqual(areEqual, new KeySequence(mod, kChar, KC.None).IsShiftImplied());
	}

	[TestMethod]
	[DataRow(	KM.None,						null,		KC.None,		"")] // Empty
	//																		//
	[DataRow(	KM.None,						"a",		KC.B,			"a")] // Lower case
	[DataRow(	KM.None,						"A",		KC.C,			"A")] // Upper case
	[DataRow(	KM.None,						"1",		KC.None,		"1")] // Number
	[DataRow(	KM.None,						"`",		KC.None,		"`")] // Special character
	[DataRow(	KM.None,						"!",		KC.None,		"!")] // Another special character
	[DataRow(	KM.None,						"§",		KC.None,		"§")] // Even more special character
	//																		//
	[DataRow(	KM.None,						null,		KC.B,			"B")] // Key code
	[DataRow(	KM.None,						null,		KC.C,			"C")] // Key code
	//																		//
	[DataRow(	KM.Alt,							"a",		KC.B,			"Alt+a")] // Key char has precedence
	[DataRow(	KM.Alt,							"A",		KC.C,			"Alt+A")] // Key char has precedence
	//																		//
	[DataRow(	KM.Alt,							"B",		KC.None,		"Alt+B")] // Modifier + key char + key code
	[DataRow(	KM.Alt,							"C",		KC.None,		"Alt+C")] // Modifier + key char + key code
	[DataRow(	KM.Alt,							null,		KC.B,			"Alt+B")] // Modifier + key code
	[DataRow(	KM.Alt,							null,		KC.C,			"Alt+C")] // Modifier + key code
	//																		//
	[DataRow(	KM.Alt,							null,		KC.A,			"Alt+A")] // Modifier + key code
	[DataRow(	KM.Control,						null,		KC.A,			"Control+A")] // Modifier + key code
	[DataRow(	KM.Numpad,						null,		KC.A,			"Numpad+A")] // Modifier + key code
	[DataRow(	KM.Shift,						null,		KC.A,			"Shift+A")] // Modifier + key code
	[DataRow(	KM.Super,						null,		KC.A,			"Super+A")] // Modifier + key code
	//																		//
	[DataRow(	KM.Control | KM.Alt,			null,		KC.A,			"Control+Alt+A")] // Multiple modifiers
	[DataRow(	KM.Control | KM.Numpad,			null,		KC.A,			"Control+Numpad+A")] // Multiple modifiers
	[DataRow(	KM.Control | KM.Shift,			null,		KC.A,			"Control+Shift+A")] // Multiple modifiers
	[DataRow(	KM.Control | KM.Super,			null,		KC.A,			"Control+Super+A")] // Multiple modifiers
	public void ToShortStringTest(KM mod, string? kChar, KC kCode, string asString)
	{
		Assert.AreEqual(asString, new KeySequence(mod, kChar, kCode).ToShortString());
	}
}