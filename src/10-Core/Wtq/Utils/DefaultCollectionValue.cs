namespace Wtq.Utils;

/// <summary>
/// Version of <see cref="DefaultValueAttribute"/>, but for collections.
/// </summary>
public sealed class DefaultCollectionValue(object[] values) : Attribute
{
	public object[] Values { get; } = Guard.Against.Null(values);
}