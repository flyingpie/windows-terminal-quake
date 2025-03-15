namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class SystemExtensionsTest
{
	[TestMethod]
	[DataRow(null, null)]
	[DataRow("", null)]
	[DataRow(" ", null)]
	[DataRow("\t", null)]
	[DataRow("not-empty", "not-empty")]
	public void EmptyOrWhiteSpaceToNull(string inp, string expected)
	{
		Assert.AreEqual(expected, inp.EmptyOrWhiteSpaceToNull());
	}
}