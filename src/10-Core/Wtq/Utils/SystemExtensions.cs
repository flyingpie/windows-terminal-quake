using Namotion.Reflection;
using System.Reflection;
using System.Xml.Linq;

namespace Wtq.Utils;

public static class SystemExtensions
{
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
			return translationFunction(enumVal);

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
		ArgumentNullException.ThrowIfNull(values);

		return string.Join(separator, values);
	}

	public static IEnumerable<ValidationResult> Validate(this IValidatableObject validatable)
		=> validatable.Validate(new ValidationContext(new object()));


	// public static bool IsValid(this IValidatableObject validatable)
	// 	=> !validatable.Validate().Any();
	//
	// public static bool IsValid(this IValidatableObject validatable, Expression<Func<WtqAppOptions, object>> expr)
	// 	=> !validatable.ValidationResultsFor(expr).Any();
	//
	// public static bool IsValid(this IValidatableObject validatable, string componentName)
	// 	=> !validatable.ValidationResultsFor(componentName).Any();
	//

	//
	// public static IEnumerable<ValidationResult> ValidationResultsFor(this IValidatableObject validatable, Expression<Func<WtqAppOptions, object>> expr)
	// 	=> validatable.ValidationResultsFor(GetMemberName(expr));
	//
	// public static IEnumerable<ValidationResult> ValidationResultsFor(this IValidatableObject validatable, string componentName)
	// 	=> validatable.Validate().Where(v => v.MemberNames.Any(m => m.Equals(componentName, StringComparison.Ordinal)));
	//
	//
	// public static string GetMemberName(this Expression expression)
	// {
	// 	if (expression == null)
	// 	{
	// 		var dbg = 2;
	// 	}
	//
	// 	switch (expression.NodeType)
	// 	{
	// 		case ExpressionType.Convert:
	// 			return GetMemberName(((UnaryExpression)expression).Operand);
	// 		case ExpressionType.Lambda:
	// 			return GetMemberName(((LambdaExpression)expression).Body);
	// 		case ExpressionType.MemberAccess:
	// 			return ((MemberExpression)expression).Member.Name;
	// 		default:
	// 			Console.WriteLine($"Unsupported node type '{expression}' => '{expression.NodeType}'.");
	// 			return "";
	// 			throw new NotSupportedException(expression.NodeType.ToString());
	// 	}
	// }

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

	public static string GetMemberDocEnum<TEnum>(object val)
	{
		var mem = typeof(TEnum).GetMember(val.ToString()).FirstOrDefault();
		// var m = SystemExtensions.GetMemberInfo(expr);
		//
		var x = mem.GetXmlDocsElement(new XmlDocsOptions()
		{
			FormattingMode = XmlDocsFormattingMode.Html
		});

		// return m.GetXmlDocsSummary();

		return x
				?.Descendants("summary") // TODO: Use FormattingMode on GetXmlDocs instead.
				?.FirstOrDefault()
				?.ToString(SaveOptions.None)
			?? string.Empty;
	}

	public static string GetMemberDoc(Expression expr)
	{
		var m = SystemExtensions.GetMemberInfo(expr);

		var x = m.GetXmlDocsElement();

		// return m.GetXmlDocsSummary();

		return x
				?.Descendants("summary") // TODO: Use FormattingMode on GetXmlDocs instead.
				?.FirstOrDefault()
				?.ToString(SaveOptions.None)
			?? string.Empty;
	}

	public static MemberInfo GetMemberInfo(this Expression expression)
	{
		if (expression == null)
		{
			var dbg = 2;
		}

		switch (expression.NodeType)
		{
			case ExpressionType.Convert:
				return GetMemberInfo(((UnaryExpression)expression).Operand);
			case ExpressionType.Lambda:
				return GetMemberInfo(((LambdaExpression)expression).Body);
			case ExpressionType.MemberAccess:
				return ((MemberExpression)expression).Member;
			default:
				// Console.WriteLine($"Unsupported node type '{expression}' => '{expression.NodeType}'.");
				return null;
			// throw new NotSupportedException(expression.NodeType.ToString());
		}
	}

	public static void SetPropertyValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda, TValue value)
	{
		var memberSelectorExpression = memberLamda.Body as MemberExpression;
		if (memberSelectorExpression != null)
		{
			var property = memberSelectorExpression.Member as PropertyInfo;
			if (property != null)
			{
				property.SetValue(target, value, null);
			}
		}
	}
}

public class Box<TValue>
{
	public TValue Value { get; set; }
}