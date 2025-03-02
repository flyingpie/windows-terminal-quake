using Namotion.Reflection;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace Wtq.Utils;

public static class SystemExtensions
{
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

	public static string GetMemberDoc(Expression expr)
	{
		var m = SystemExtensions.GetMemberInfo(expr);

		var x = m.GetXmlDocsElement();

		// return m.GetXmlDocsSummary();

		return x.Descendants("summary").FirstOrDefault().ToString(SaveOptions.None);
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