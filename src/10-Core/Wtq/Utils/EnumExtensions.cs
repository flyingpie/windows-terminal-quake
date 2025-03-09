namespace Wtq.Utils;

public static class EnumExtensions
{
	/// <summary>
	/// Returns attribute of tyep <typeparamref name="TAttr"/>, from enum of type <typeparamref name="TEnum"/>.<br/>
	/// Returns null if no such attribute was found.
	/// </summary>
	public static TAttr? GetAttribute<TEnum, TAttr>(this TEnum enumValue)
		where TEnum : struct
		where TAttr : Attribute
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString()!);

		if (fieldInfo == null)
		{
			return null;
		}

		var attrs = fieldInfo.GetCustomAttributes(typeof(TAttr), true);
		if (attrs != null && attrs.Length > 0)
		{
			return (TAttr)attrs[0];
		}

		return null;
	}

	public static IEnumerable<EnumItem<TEnum>> GetEnumItems<TEnum>(bool isVisibleOnly = true)
		where TEnum : struct, Enum
	{
		var list = new List<EnumItem<TEnum>>();
		var vals = Enum.GetValues<TEnum>();

		foreach (var val in vals)
		{
			var displ = val.GetAttribute<DisplayAttribute>();
			var flags = val.GetAttribute<DisplayFlagsAttribute>();

			if (isVisibleOnly && flags != null && !flags.IsVisible)
			{
				continue;
			}

			list.Add(new EnumItem<TEnum>()
			{
				Display = displ,
				Flags = flags,
				Value = val,
			});
		}

		return list;
	}
}

public class EnumItem<TValue>
{
	public DisplayFlagsAttribute? Flags { get; set; }

	public DisplayAttribute? Display { get; set; }

	public bool IsVisible => Flags?.IsVisible ?? true;

	public TValue Value { get; set; }

	public string DisplayName => Display?.Name ?? Value.ToString();

	public string Doc => SystemExtensions.GetMemberDocEnum<TValue>(Value);
}