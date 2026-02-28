namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class EnumUtilsTest
{
	private enum TestEnum
	{
		None = 0,

		MyValue1 = 1,
		MyValue2 = 2,
	}

	[TestMethod]
	public void TryParseTest()
	{
		// Name 1
		Assert.IsTrue(EnumUtils.TryParse<TestEnum>("MyValue1", ignoreCase: true, out var val1));
		Assert.AreEqual(TestEnum.MyValue1, val1);

		// Name 2
		Assert.IsTrue(EnumUtils.TryParse<TestEnum>("MyValue2", ignoreCase: true, out var val2));
		Assert.AreEqual(TestEnum.MyValue2, val2);

		// By value (should fail)
		Assert.IsFalse(EnumUtils.TryParse<TestEnum>("1", ignoreCase: true, out _));
	}
}