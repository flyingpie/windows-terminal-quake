namespace Wtq.Utils;

public sealed class ExampleValueAttribute : Attribute
{
	public string Value { get; }

	public ExampleValueAttribute(string value)
	{
		Value = value;
	}
}