namespace Wtq.Utils;

public static class EnumUtils
{
	public static IEnumerable<EnumValue> GetValues(Type enumType, bool isVisibleOnly = true)
	{
		var list = new List<EnumValue>();
		var vals = Enum.GetValues(enumType);

		foreach (var val in vals)
		{
			var enumVal = (Enum)val;
			var displ = enumVal.GetAttribute<DisplayAttribute>();
			var flags = enumVal.GetAttribute<DisplayFlagsAttribute>();

			if (isVisibleOnly && flags != null && !flags.IsVisible)
			{
				continue;
			}

			list.Add(new EnumValue()
			{
				Display = displ, Flags = flags, Value = val,
			});
		}

		return list;
	}

	public static IEnumerable<EnumValue<TEnum>> GetValues<TEnum>(bool isVisibleOnly = true)
		where TEnum : struct, Enum
	{
		var list = new List<EnumValue<TEnum>>();
		var vals = Enum.GetValues<TEnum>();

		foreach (var val in vals)
		{
			var displ = val.GetAttribute<DisplayAttribute>();
			var flags = val.GetAttribute<DisplayFlagsAttribute>();

			if (isVisibleOnly && flags != null && !flags.IsVisible)
			{
				continue;
			}

			list.Add(new EnumValue<TEnum>()
			{
				Display = displ, Flags = flags, Value = val,
			});
		}

		return list;
	}
}