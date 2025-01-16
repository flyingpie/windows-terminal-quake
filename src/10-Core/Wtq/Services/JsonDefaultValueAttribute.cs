namespace Wtq.Services;

public class JsonDefaultValueAttribute(object? defaultValue) : Attribute
{
	public object? DefaultValue { get; } = defaultValue;
}
