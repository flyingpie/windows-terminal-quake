using Namotion.Reflection;
using System.Reflection;

namespace Wtq.Utils;

public static class XmlDocUtils
{
	private static readonly XmlDocsOptions _xmlDocsOptions = new()
	{
		FormattingMode = XmlDocsFormattingMode.Html,
	};

	public static string? GetSummary(this MemberInfo memberInfo)
	{
		Guard.Against.Null(memberInfo);

		return Guard.Against.Null(memberInfo).GetXmlDocsTag("summary", _xmlDocsOptions)?.Replace("\n", "<br/>");
	}

	public static string? GetSummaryEnum(this object val, Type enumType)
	{
		Guard.Against.Null(val);
		Guard.Against.Null(enumType);

		return enumType.GetMember(val.ToString()!).FirstOrDefault().GetSummary()
			?? throw new InvalidOperationException($"Could not get member info for enum type '{enumType.FullName}'.");
	}
}