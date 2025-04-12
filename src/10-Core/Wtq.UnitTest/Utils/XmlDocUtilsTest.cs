using Namotion.Reflection;
using System.Linq;

namespace Wtq.Core.UnitTest.Utils;

[TestClass]
public class XmlDocUtilsTest
{
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
		/// 	"Name": "Value"
		/// }
		/// </code>
		/// </example>
		public string? Property { get; set; }
	}

	[TestMethod, Ignore]
	public void METHOD()
	{
		var classType = typeof(TestClass);
		var propType = classType.GetProperty(nameof(TestClass.Property));

		var summary = propType.GetSummary();

		Assert.AreEqual("""
			The Summary.
			Another line.
			<p>With <strong>strong</strong></p> words.
			""", summary);

		// var doc = XmlDocUtils
		// 	.GetMemberDocElement(propType)
		// 	.Descendants("summary")
		// 	.FirstOrDefault();

		var dbg = 2;
	}
}