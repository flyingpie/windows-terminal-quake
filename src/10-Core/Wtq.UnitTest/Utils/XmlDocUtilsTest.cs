using System.Reflection;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class XmlDocUtilsTest
{
	[TestMethod]
	public void GetExample()
	{
		// Act
		var summary = TestClass.PropertyInfo.GetExample().ReplaceLineEndings();

		// Assert
		Assert.AreEqual(
			"""
			{
			  "Name": "Value"
			}
			""",
			summary);
	}

	[TestMethod]
	public void GetRemarks()
	{
		// Act
		var summary = TestClass.PropertyInfo.GetRemarks().ReplaceLineEndings();

		// Assert
		Assert.AreEqual(
			"""
			The Remarks.
			Another line.
			<p>With paragraph.</p>
			With <strong>strong</strong> words.
			""",
			summary);
	}

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
			With <strong>strong</strong> words.
			""",
			summary);
	}

	private class TestClass
	{
		/// <summary>
		/// The Summary.<br/>
		/// Another line.
		/// <para>With paragraph.</para>
		/// With <strong>strong</strong> words.
		/// </summary>
		/// <remarks>
		/// The Remarks.<br/>
		/// Another line.
		/// <para>With paragraph.</para>
		/// With <strong>strong</strong> words.
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