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
			(new(KM.None,								null,	KC.A),					"A"),

			(new(KM.None,								"A",	null),					"A"),
			(new(KM.Control,							"1",	null),					"Ctrl+1"),
			(new(KM.Control | KM.Numpad,				"1",	null),					"Ctrl+Num+1"),

			(new(KM.Control,							"§",	null),					"Ctrl+§"),
			(new(KM.Control,							"§",	null),					"Ctrl+§"),

			// @formatter:on
#pragma warning restore SA1027
		};

		foreach (var x in l)
		{
			// Assert.AreEqual(x.Sequence.ToKWin(), x.KWin);
		}
	}
}