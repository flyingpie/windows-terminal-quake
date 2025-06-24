using Wtq.Input;

namespace Wtq.Core.UnitTest.Input;

[TestClass]
public class KeySequenceTest
{
	[TestMethod]
	public void METHOD()
	{
		Assert.IsTrue(new KeySequence(KeyModifiers.None, null, KeyCode.A) == new KeySequence(KeyModifiers.None, null, KeyCode.A));
	}
}