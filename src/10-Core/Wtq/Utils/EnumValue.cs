namespace Wtq.Utils;

/// <summary>
/// Wrapper for enum values, where we can include additional data such as display name.
/// </summary>
public class EnumValue<TValue>
	where TValue : struct
{
	private string? _displayName;
	private string? _doc;

	public required TValue Value { get; init; }

	public DisplayAttribute? Display { get; set; }

	public DisplayFlagsAttribute? Flags { get; set; }

	public string? DisplayName => _displayName ??= Display?.Name ?? Value.ToString();

	public string? Doc => _doc ??= XmlDocUtils.GetSummaryEnum(Value, typeof(TValue));
}

public class EnumValue
{
	private string? _displayName;
	private string? _doc;

	public required object Value { get; init; }

	public DisplayAttribute? Display { get; set; }

	public DisplayFlagsAttribute? Flags { get; set; }

	public string? DisplayName => _displayName ??= Display?.Name ?? Value.ToString();

	public string? DocSummary => _doc ??= XmlDocUtils.GetSummaryEnum(Value, Value.GetType());
}
