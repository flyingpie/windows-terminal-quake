using Namotion.Reflection;
using System.Reflection;
using System.Xml.Linq;

namespace Wtq.Utils;

public static class XmlDocUtils
{
	private static readonly XmlDocsOptions _xmlDocsOptions = new()
	{
		FormattingMode = XmlDocsFormattingMode.Html,
	};

	public static string? GetExample(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo)
			?.GetXmlDocsElement()
			?.Element("example")
			?.ToString()
			?.Replace("             ", "")
			?.Replace("<example>", "")
			?.Replace("</example>", "")
			?.Replace("<code>", "```")
			?.Replace("<code lang=\"json\">", "```json")
			?.Replace("</code>", "```")
			?? string.Empty;

	public static string? GetRemarks(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("remarks", _xmlDocsOptions);

	public static string? GetSummary(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("summary", _xmlDocsOptions);

	public static string? GetSummaryEnum(this object val, Type enumType)
	{
		var member = enumType.GetMember(val.ToString()!).FirstOrDefault()
			?? throw new InvalidOperationException($"Could not get member info for enum type '{enumType.FullName}'.");

		return GetSummary(member);
	}
}