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

	[TestMethod]
	[DataRow("the-filename", "the-filename")]
	[DataRow("the-filename.exe", "the-filename")]
	public void GetFileNameWithoutExtension(string fileName, string expected)
	{
		Assert.AreEqual(expected, fileName.GetFileNameWithoutExtension());
	}

	[TestMethod]
	[DataRow("")]
	[DataRow(" ")]
	[DataRow("\t")]
	public void GetFileNameWithoutExtensionEmpty(string fileName)
	{
		Assert.ThrowsExactly<ArgumentException>(fileName.GetFileNameWithoutExtension);
	}

	[TestMethod]
	[DataRow(null)]
	public void GetFileNameWithoutExtensionNull(string fileName)
	{
		Assert.ThrowsExactly<ArgumentNullException>(fileName.GetFileNameWithoutExtension);
	}

	[TestMethod]
	[DataRow("", "")]
	[DataRow(" ", " ")]
	[DataRow("a", "a")]
	[DataRow("A", "a")]
	[DataRow("aa", "aa")]
	[DataRow("Aa", "aa")]
	[DataRow("AA", "a_a")]
	[DataRow("Snake", "snake")]
	[DataRow("SNAKE", "s_n_a_k_e")]
	[DataRow("ToSnakeCase", "to_snake_case")]
	[DataRow("to_snake_case", "to_snake_case")]
	[DataRow("To_Snake_Case", "to__snake__case")]
	public void ToSnakeCase(string inp, string expected)
	{
		Assert.AreEqual(expected, inp.ToSnakeCase());
	}

	[TestMethod]
	public void ToSnakeCaseNull()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => ((string)null)!.ToSnakeCase());
	}
}