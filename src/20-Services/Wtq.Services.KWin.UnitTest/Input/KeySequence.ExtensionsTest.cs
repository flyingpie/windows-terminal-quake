using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wtq.Input;
using KC = Wtq.Input.KeyCode;
using KM = Wtq.Input.KeyModifiers;

namespace Wtq.Services.KWin.UnitTest.Input;

[TestClass]
public class KeySequenceExtensionsTest
{
	[TestMethod]
	public void METHOD()
	{
		var l = new List<(KeySequence Sequence, string KWin)>()
		{
		// @formatter:off
#pragma warning disable SA1027

			// Single keys
			(new(KM.None,								null,	KC.A),					"A"),

			(new(KM.None,								"A",	KC.None),				"A"),
			(new(KM.Control,							"1",	KC.None),				"Ctrl+1"),
			(new(KM.Control | KM.Numpad,				"1",	KC.None),				"Ctrl+Num+1"),

			(new(KM.Control,							"§",	KC.None),				"Ctrl+§"),
			(new(KM.Control,							"§",	KC.None),				"Ctrl+§"),

			// @formatter:on
#pragma warning restore SA1027
		};

		foreach (var x in l)
		{
			// Assert.AreEqual(x.Sequence.ToKWin(), x.KWin);
		}
	}
}