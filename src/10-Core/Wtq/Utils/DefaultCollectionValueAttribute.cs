namespace Wtq.Utils;

/// <summary>
/// Version of <see cref="DefaultValueAttribute"/>, but for collections.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DefaultCollectionValueAttribute(object[] values) : Attribute
{
	public ICollection<object> Values { get; } = Guard.Against.Null(values);
}