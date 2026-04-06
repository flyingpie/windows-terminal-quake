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
		Assert.AreEqual(
			"""
			The Summary.
			Another line.
			<p>With paragraph.</p>
			With <b>strong</b> words.
			""",
			summary);
	}

	[TestMethod]
	public void GetSummaryEnum()
	{
		Assert.Inconclusive();
	}

	private class TestClass
	{
		/// <summary>
		/// The Summary.<br/>
		/// Another line.
		/// <para>With paragraph.</para>
		/// With <b>strong</b> words.
		/// </summary>
		/// <remarks>
		/// The Remarks.<br/>
		/// Another line.
		/// <para>With paragraph.</para>
		/// With <b>strong</b> words.
		/// </remarks>
		/// <example>
		/// <code>
		/// {
		///   "Name": "Value"
		/// }
		/// </code>
		/// </example>
		public string? Property { get; set; }

		public static PropertyInfo PropertyInfo => typeof(TestClass).GetProperty(nameof(Property))!;
	}
}