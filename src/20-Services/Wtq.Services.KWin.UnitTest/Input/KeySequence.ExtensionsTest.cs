using Wtq.Input;
using Wtq.Services.KWin.Input;
using KC = Wtq.Input.KeyCode;
using KM = Wtq.Input.KeyModifiers;

namespace Wtq.Services.KWin.UnitTest.Input;

[TestClass]
public class KeySequenceExtensionsTest
{
	[TestMethod]

	// @formatter:off
#pragma warning disable SA1027

	// Single keys
	[DataRow(KM.None,								null,	KC.A,					"A")]

	[DataRow(KM.None,								"A",	KC.None,				"A")]
	[DataRow(KM.Control,							"1",	KC.None,				"Ctrl+1")]
	[DataRow(KM.Control,							"9",	KC.None,				"Ctrl+9")]
	[DataRow(KM.Control | KM.Numpad,				"1",	KC.None,				"Ctrl+Num+1")]

	// Implied "SHIFT"
	[DataRow(KM.Control | KM.Shift,					"§",	KC.None,				"Ctrl+§")]

	[DataRow(KM.Control | KM.Shift,					"a",	KC.None,				"Ctrl+Shift+A")]
	[DataRow(KM.Control | KM.Shift,					"A",	KC.None,				"Ctrl+Shift+A")]

	// @formatter:on
#pragma warning restore SA1027
	public void ToKWinStringTest(KM mod, string keyChar, KC keyCode, string expectedKWinString)
	{
		Assert.AreEqual(expectedKWinString, new KeySequence(mod, keyChar, keyCode).ToKWinString());
	}
}