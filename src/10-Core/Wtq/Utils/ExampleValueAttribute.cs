namespace Wtq.Utils;

public sealed class ExampleValueAttribute : Attribute
{
	public object Value { get; }

	public ExampleValueAttribute(object value)
	{
		Value = value;
	}
}