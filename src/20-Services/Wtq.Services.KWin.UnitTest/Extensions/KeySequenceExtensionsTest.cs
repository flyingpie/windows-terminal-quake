using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Wtq.Configuration;
using K = Wtq.Configuration.Keys;
using KM = Wtq.Configuration.KeyModifiers;

namespace Wtq.Services.KWin.UnitTest.Extensions;

[TestClass]
public class KeySequenceExtensionsTest
{
	[TestMethod]
	public void METHOD()
	{
		// @formatter:off

		var l = new List<(KeySequence Sequence, string KWin)>()
		{
			(new(KM.None,								null,	K.A),					"A"),

			(new(KM.None,								"A",	null),					"A"),
			(new(KM.Control,							"1",	null),					"Ctrl+1"),
			(new(KM.Control | KM.Numpad,				"1",	null),					"Ctrl+Num+1"),
		};

		// @formatter:on

		foreach (var x in l)
		{
			Assert.AreEqual(x.Sequence.ToKWin(), x.KWin);
		}
	}
}