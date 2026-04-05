using Namotion.Reflection;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Wtq.Utils;

public static class XmlDocUtils
{
	private static readonly XmlDocsOptions _xmlDocsOptions = new()
	{
		FormattingMode = XmlDocsFormattingMode.Html,
	};

	public static string? GetExample(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsTag("example", _xmlDocsOptions);

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

	private static readonly Regex _aHrefRegex = new(@"<a\b[^>]*>(.*?)</a>", RegexOptions.Compiled);

	public static string RemoveLinks(this string source)
	{
		Guard.Against.Null(source);

		// var result = Regex.Replace(source, @"<a\b[^>]*>(.*?)</a>", "$1", RegexOptions.IgnoreCase);

		return _aHrefRegex.Replace(source, "$1");
	}
}