using System.Xml.Linq;

namespace Wtq.Utils;

/// <summary>
/// Wrapper for enum values, where we can include additional data such as display name.
/// </summary>
public class EnumValue<TValue>
	where TValue : struct
{
	private string? _displayName;
	private string? _doc;

	public TValue Value { get; set; }

	public DisplayAttribute? Display { get; set; }

	public DisplayFlagsAttribute? Flags { get; set; }

	public string? DisplayName => _displayName ??= Display?.Name ?? Value.ToString();

	public string? Doc => _doc ??= Value.GetMemberDocEnum<TValue>();
}

public class EnumValue
{
	private string? _displayName;
	private string? _doc;

	// public Type EnumType { get; set; }

	public object Value { get; set; }

	public DisplayAttribute? Display { get; set; }

	public DisplayFlagsAttribute? Flags { get; set; }

	public string? DisplayName => _displayName ??= Display?.Name ?? Value.ToString();

	public string? Doc => _doc ??= Value.GetMemberDocEnum(Value.GetType());

	public XElement? DocElement => Value.GetMemberDocEnumElement(Value.GetType());
}
