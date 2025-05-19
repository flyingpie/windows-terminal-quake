using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class XmlDocUtilsTest
{
	[TestMethod]
	public void GetExample()
	{
		// Act
		var summary = TestClass.PropertyInfo.GetExample().ReplaceLineEndings();
		// var x = TestClass.PropertyInfo.GetExampleX().Element("example");
		// var yy = x.ToString();
			//.Replace("<example>", "").Replace("</example>", "");


		// var m = yy.Split("\n");
		// var nn = m.Last();
		// var s = nn.Count(l => l == ' ');

		// var rea = new StringReader(yy);

		// var y2 = yy.Replace("\n    ", "");

		// var n = Regex.Match(yy, "(.*)</summary>");

		// var sb = new StringBuilder();
		// foreach (var y in x.Element("example").Elements())
		// {
		// 	sb.AppendLine(y.ToString());
		//
		// 	var xx = 2;
		// }


		var dbg = 2;

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
		/// Globally:
		/// <code lang="json">
		/// {
		/// 	"Name": "Value"
		/// }
		/// </code>
		///
		/// For one app only:
		/// <code>
		/// {
		/// 	"Name": "Value"
		/// }
		/// </code>
		/// </example>
		public string? Property { get; set; }

		public static PropertyInfo PropertyInfo => typeof(TestClass).GetProperty(nameof(Property))!;
	}
}