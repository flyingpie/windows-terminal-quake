namespace Wtq.Core.UnitTest.Configuration;

[TestClass]
public class KeySequenceTest
{
	[TestMethod]
	public void METHOD()
	{
		Assert.IsTrue(new KeySequence(KeyModifiers.None, null, Keys.A) == new KeySequence(KeyModifiers.None, null, Keys.A));
	}
}