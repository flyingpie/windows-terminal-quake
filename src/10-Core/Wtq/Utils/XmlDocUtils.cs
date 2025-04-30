using Namotion.Reflection;
using System.Reflection;

namespace Wtq.Utils;

public static class XmlDocUtils
{
	private static XmlDocsOptions _xmlDocsOptions = new()
	{
		FormattingMode = XmlDocsFormattingMode.Html,
	};

	public static string? GetExample(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("example", _xmlDocsOptions);

	public static string? GetRemarks(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("remarks", _xmlDocsOptions);

	public static string? GetSummary(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("summary", _xmlDocsOptions);

	#region Enums

	public static string? GetSummaryEnum(this object val, Type enumType)
	{
		var memb = enumType.GetMember(val.ToString()!).First();

		return GetSummary(memb);
	}

	#endregion
}