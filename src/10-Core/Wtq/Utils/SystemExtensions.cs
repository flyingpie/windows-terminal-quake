using System.Linq.Expressions;

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


	public static bool IsValid(this IValidatableObject validatable) => !validatable.Validate().Any();

	public static bool IsValid(this IValidatableObject validatable, Expression<Func<WtqAppOptions, object>> expr)
		=> !validatable.ValidationResultsFor(expr).Any();

	public static bool IsValid(this IValidatableObject validatable, string componentName)
		=> !validatable.ValidationResultsFor(componentName).Any();

	public static IEnumerable<ValidationResult> Validate(this IValidatableObject validatable)
		=> validatable.Validate(new ValidationContext(new object()));

	public static IEnumerable<ValidationResult> ValidationResultsFor(this IValidatableObject validatable, Expression<Func<WtqAppOptions, object>> expr)
		=> validatable.ValidationResultsFor(GetMemberName(expr));

	public static IEnumerable<ValidationResult> ValidationResultsFor(this IValidatableObject validatable, string componentName)
		=> validatable.Validate().Where(v => v.MemberNames.Any(m => m.Equals(componentName, StringComparison.Ordinal)));


	public static string GetMemberName(Expression expression)
	{
		switch (expression.NodeType)
		{
			case ExpressionType.Convert:
				return GetMemberName(((UnaryExpression)expression).Operand);
			case ExpressionType.Lambda:
				return GetMemberName(((LambdaExpression)expression).Body);
			case ExpressionType.MemberAccess:
				return ((MemberExpression)expression).Member.Name;
			default:
				throw new NotSupportedException(expression.NodeType.ToString());
		}
	}
}