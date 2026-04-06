namespace Wtq.Utils;

/// <summary>
/// Wrapper for enum values, where we can include additional data such as display name.
/// </summary>
public class EnumValue<TValue>
	where TValue : struct
{
	public required TValue Value { get; init; }

	public DisplayAttribute? Display { get; set; }

	public string? DisplayName => field ??= Display?.Name ?? Value.ToString();

	public string? Summary => field ??= Value.GetSummaryEnum(typeof(TValue));
}