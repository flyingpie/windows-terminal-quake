namespace Wtq.Input;

[AttributeUsage(AttributeTargets.Enum)]
public sealed class WebKeyAttribute : Attribute
{
	public string? Key { get; set; }

	public string? Code { get; set; }
}