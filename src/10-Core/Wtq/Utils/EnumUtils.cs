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

	/// <summary>
	/// Attempts to parse the specified <paramref name="name"/> as an enum _name_.<br/>
	/// Difference with regular Enum.TryParse(), is that this does not consider enum _values_.<br/>
	/// <br/>
	/// For example, the name for KeyCode enum "Tab" has value "0x09". Calling Enum.TryParse() with type KeyCode, and value "9", will return "Tab".<br/>
	/// This version only considers the names, not the values.
	/// </summary>
	public static bool TryParse<TEnum>([NotNullWhen(true)] string? name, bool ignoreCase, out TEnum result)
		where TEnum : struct
	{
		result = default;

		return

			// Skip parsing if the key character is numeric itself (as that will never map to an enum name, i.e. enum names
			// must always start with a letter, so pure numbers will not work).
			// This prevents key characters from being parsed through _value_ of the enum (e.g. "9" => "Tab", because Tab = 0x09).
			!int.TryParse(name, out _) &&

			// Now try to parse the enum.
			Enum.TryParse(name, ignoreCase: ignoreCase, out result);
	}
}