namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class SystemExtensionsTest
{
	[TestMethod]
	public void StringJoin_Empty()
	{
		Assert.AreEqual("", Array.Empty<string>().StringJoin());
	}

	[TestMethod]
	public void StringJoin_Single()
	{
		Assert.AreEqual("String 1", new[] { "String 1" }.StringJoin());
	}

	[TestMethod]
	public void StringJoin_Multiple()
	{
		Assert.AreEqual("String 1, String 2", new[] { "String 1", "String 2" }.StringJoin());
	}

	[TestMethod]
	public void StringJoin_Delimiter()
	{
		Assert.AreEqual("String 1 | String 2", new[] { "String 1", "String 2" }.StringJoin(" | "));
	}
}