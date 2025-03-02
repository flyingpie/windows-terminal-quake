namespace Wtq.Utils;

[AttributeUsage(AttributeTargets.Property)]
public sealed class JsonDefaultValue : Attribute
{
	public JsonDefaultValue(object value)
	{
		Value = value;
	}

	public object Value { get; set; }
}