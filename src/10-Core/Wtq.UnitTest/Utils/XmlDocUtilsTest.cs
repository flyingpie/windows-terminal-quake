using System.Linq;
using System.Reflection;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class XmlDocUtilsTest
{
	[TestMethod]
	public void GetSummary()
	{
		// Act
		var summary = TestClass.PropertyInfo.GetSummary().ReplaceLineEndings();

		// Assert
		Assert.AreEqual("The Summary.<br/>Another line.<br/>With <strong>strong</strong> words.", summary);
	}

	[TestMethod]
	public void GetSummaryEnum()
	{
		// Act
		var summaries = EnumUtils.GetValues<TestEnum>().ToList();
		Assert.HasCount(1, summaries);

		var summary = summaries[0];

		// Assert
		Assert.AreEqual("The Summary.<br/>Another line.<br/>With <strong>strong</strong> words.", summary.Summary);
	}

	private class TestClass
	{
		/// <summary>
		/// The Summary.<br/>
		/// Another line.
		/// With <b>strong</b> words.
		/// </summary>
		public string? Property { get; set; }

		public static PropertyInfo PropertyInfo => typeof(TestClass).GetProperty(nameof(Property))!;
	}

	public enum TestEnum
	{
		/// <summary>
		/// The Summary.<br/>
		/// Another line.
		/// With <b>strong</b> words.
		/// </summary>
		EnumValue1,
	}
}