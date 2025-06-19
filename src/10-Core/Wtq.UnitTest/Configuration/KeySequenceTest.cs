namespace Wtq.Core.UnitTest.Configuration;

[TestClass]
public class KeySequenceTest
{
	[TestMethod]
	public void METHOD()
	{
		Assert.IsTrue(new KeySequence(KeyModifiers.None, Keys.A, null) == new KeySequence(KeyModifiers.None, Keys.A, null));
	}
}