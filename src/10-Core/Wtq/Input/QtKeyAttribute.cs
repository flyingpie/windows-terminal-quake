namespace Wtq.Input;

[AttributeUsage(AttributeTargets.Field)]
public sealed class QtKeyAttribute(string name) : Attribute
{
	public string Name { get; } = Guard.Against.NullOrWhiteSpace(name);
}