using Namotion.Reflection;
using System.Reflection;
using System.Xml.Linq;

namespace Wtq.Utils;

public static class AttrUtils
{
	/// <summary>
	/// A generic extension method that aids in reflecting
	/// and retrieving any attribute that is applied to an `Enum`.
	/// </summary>
	public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
		where TAttribute : Attribute
	{
		Guard.Against.Null(enumValue);

		return enumValue
			.GetType()
			.GetMember(enumValue.ToString())
			.First()
			.GetCustomAttribute<TAttribute>();
	}

	/// <summary>
	/// Returns attribute of type <typeparamref name="TAttr"/>, from enum of type <typeparamref name="TEnum"/>.<br/>
	/// Returns null if no such attribute was found.
	/// </summary>
	public static TAttr? GetAttribute<TEnum, TAttr>(this TEnum enumValue)
		where TEnum : struct
		where TAttr : Attribute
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString()!);

		if (fieldInfo == null)
		{
			return null;
		}

		var attrs = fieldInfo.GetCustomAttributes(typeof(TAttr), true);
		if (attrs != null && attrs.Length > 0)
		{
			return (TAttr)attrs[0];
		}

		return null;
	}

	public static object GetDefaultValueFor(this Expression expr)
	{
		var val = Guard.Against.Null(expr).GetMemberInfo()?.GetCustomAttribute<DefaultValueAttribute>()?.Value;
		if (val != null)
		{
			return val;
		}

		return default;
	}

	public static TValue GetDefaultValueFor<TValue>(this Expression expr)
	{
		var val = Guard.Against.Null(expr).GetMemberInfo()?.GetCustomAttribute<DefaultValueAttribute>()?.Value;
		if (val != null)
		{
			return (TValue)val;
		}

		return default;
	}

	public static string? GetDisplayName(this Expression expr) =>
		Guard.Against.Null(expr).GetDisplayAttr()?.Name ?? expr.GetMemberInfo()?.Name;

	public static string? GetPrompt(this Expression expr) =>
		Guard.Against.Null(expr).GetMemberInfo()?.GetCustomAttribute<DisplayAttribute>()?.Prompt;

	public static DisplayAttribute? GetDisplayAttr(this Expression expr) =>
		Guard.Against.Null(expr).GetMemberInfo()?.GetCustomAttribute<DisplayAttribute>();

	public static (int Min, int Max)? GetRange(this Expression expr)
	{
		var attr = Guard.Against.Null(expr)
			.GetMemberInfo()
			?.GetCustomAttribute<RangeAttribute>();

		if (attr == null)
		{
			return null;
		}

		return ((int)attr.Minimum, (int)attr.Maximum);
	}

	public static string? GetMemberDocEnum(this object val, Type enumType) =>
		enumType.GetMember(val.ToString()!).FirstOrDefault()?.GetMemberDoc();

	public static XElement? GetMemberDocEnumElement(this object val, Type enumType) =>
		enumType.GetMember(val.ToString()!).FirstOrDefault()?.GetMemberDocElement();

	public static string? GetMemberDocEnum<TEnum>(this object val) =>
		val.GetMemberDocEnum(typeof(TEnum));

	public static string? GetMemberDocExpr(this Expression expr) =>
		Guard.Against.Null(expr).GetMemberInfo()?.GetMemberDoc();


	public static XElement? GetMemberDocElement(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo).GetXmlDocsElement();

	public static string? GetMemberDoc(this MemberInfo memberInfo) =>
		Guard.Against.Null(memberInfo)
			.GetXmlDocsElement(new XmlDocsOptions() { FormattingMode = XmlDocsFormattingMode.Markdown, })
			// ?.Descendants("summary")
			// ?.FirstOrDefault()
			?.ToString(SaveOptions.None)
		?? string.Empty;

	public static MemberInfo? GetMemberInfo(this Expression expression)
	{
		Guard.Against.Null(expression);

		switch (expression.NodeType)
		{
			case ExpressionType.Convert:
				return GetMemberInfo(((UnaryExpression)expression).Operand);
			case ExpressionType.Lambda:
				return GetMemberInfo(((LambdaExpression)expression).Body);
			case ExpressionType.MemberAccess:
				return ((MemberExpression)expression).Member;
			default:
				return null;
		}
	}
}