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
	[DataRow(KM.None, null, KC.A, 0x00000041)]
	[DataRow(KM.None, "A", KC.None, 0x00000041)]
	[DataRow(KM.None, null, KC.F1, 0x01000030)]
	[DataRow(KM.None, null, KC.F4, 0x01000033)]
	[DataRow(KM.Control, null, KC.Escape, 0x05000000)]
	[DataRow(KM.Super, null, KC.Space, 0x10000020)]
	[DataRow(KM.Alt, null, KC.F4, 0x09000033)]
	// Key characters
	[DataRow(KM.Control, "1", KC.None, 0x04000031)]
	[DataRow(KM.Control | KM.Numpad, "1", KC.None, 0x24000031)]
	// Implied "SHIFT"
	[DataRow(KM.Control | KM.Shift, "!", KC.None, 0x04000021)]
	[DataRow(KM.Control | KM.Shift, "a", KC.None, 0x06000041)]
	[DataRow(KM.Control | KM.Shift, "A", KC.None, 0x06000041)]
	// @formatter:on
#pragma warning restore SA1027
	public void ToQtKeyCombinationTest(KM mod, string keyChar, KC keyCode, int expected)
	{
		Assert.AreEqual(expected, new KeySequence(mod, keyChar, keyCode).ToQtKeyCombination());
	}
}
