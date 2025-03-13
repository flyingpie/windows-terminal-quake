using Namotion.Reflection;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;

namespace Wtq.Utils;

public static class Opts
{
	public static TValue Cascade<TValue>(
		Expression<Func<WtqSharedOptions, object?>> expr,
		params WtqSharedOptions[] opts
	)
	{
		Guard.Against.Null(expr);

		var v = expr.Compile();

		foreach (var opt in opts)
		{
			var fromApp = v(opt);
			if (fromApp != null)
			{
				return (TValue)fromApp;
			}
		}

		return DefaultValue.For<TValue>(expr);
	}
}

public static class SystemExtensions
{
	// public static OptionsBuilder<WtqOptions> AddWtqOptionsPostConfigure(this OptionsBuilder<WtqOptions> builder)
	// {
	// 	Guard.Against.Null(builder);
	//
	// 	return builder.PostConfigure(o => o.OnPostConfigure());
	// }

	public static TValue JsonDeepClone<TValue>(this TValue value)
	{
		var json = JsonSerializer.Serialize(value);

		return JsonSerializer.Deserialize<TValue>(json)!;
	}

	public static bool HasHotkey(this ICollection<HotkeyOptions> hotkeys, Keys key, KeyModifiers modifiers)
	{
		Guard.Against.Null(hotkeys);

		return hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public static string GetDisplayName(this Enum enumValue, Func<string, string> translationFunction = null)
	{
		var enumValueAsString = enumValue.ToString();
		var val = enumValue.GetType().GetMember(enumValueAsString).FirstOrDefault();
		var enumVal = val?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? enumValueAsString;

		if (translationFunction != null)
		{
			return translationFunction(enumVal);
		}

		return enumVal;
	}

	/// <summary>
	/// A generic extension method that aids in reflecting
	/// and retrieving any attribute that is applied to an `Enum`.
	/// </summary>
	public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
		where TAttribute : Attribute
	{
		return enumValue
			.GetType()
			.GetMember(enumValue.ToString())
			.First()
			.GetCustomAttribute<TAttribute>();
	}


	public static string ExpandEnvVars(this string src)
	{
		src ??= string.Empty;

		return Environment
				.ExpandEnvironmentVariables(src)
				?.Replace("~", WtqPaths.UserHome)
			?? string.Empty;
	}

	public static string? EmptyOrWhiteSpaceToNull(this string? input)
	{
		if (string.IsNullOrWhiteSpace(input))
		{
			return null;
		}

		return input;
	}

	public static string? StringJoin<T>(this IEnumerable<T> values, string separator = ", ")
	{
		Guard.Against.Null(values);

		return string.Join(separator, values);
	}

	public static IEnumerable<ValidationResult> Validate(this IValidatableObject validatable)
	{
		Guard.Against.Null(validatable);

		return validatable.Validate(new ValidationContext(new object()));
	}

	public static string GetDisplayName(Expression expr)
	{
		var m = expr.GetMemberInfo();

		var attr = m.GetCustomAttribute<DisplayAttribute>();

		return attr?.Name ?? m.Name;
	}

	public static DisplayAttribute? GetDisplay(Expression expr)
	{
		var m = expr.GetMemberInfo();

		return m.GetCustomAttribute<DisplayAttribute>();
	}

	public static (int Min, int Max)? GetRange(Expression expr)
	{
		var m = expr.GetMemberInfo();
		var attr = m.GetCustomAttribute<RangeAttribute>();

		if (attr == null)
		{
			return null;
		}

		return ((int)attr.Minimum, (int)attr.Maximum);
	}

	public static string GetMemberDocEnum<TEnum>(object val)
	{
		var mem = typeof(TEnum).GetMember(val.ToString()).FirstOrDefault();
		var x = mem.GetXmlDocsElement(new XmlDocsOptions()
		{
			FormattingMode = XmlDocsFormattingMode.Html,
		});

		return x
				?.Descendants("summary") // TODO: Use FormattingMode on GetXmlDocs instead.
				?.FirstOrDefault()
				?.ToString(SaveOptions.None)
			?? string.Empty;
	}

	public static string GetMemberDoc(Expression expr)
	{
		var m = expr.GetMemberInfo();
		var x = m.GetXmlDocsElement();

		return x
				?.Descendants("summary") // TODO: Use FormattingMode on GetXmlDocs instead.
				?.FirstOrDefault()
				?.ToString(SaveOptions.None)
			?? string.Empty;
	}

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