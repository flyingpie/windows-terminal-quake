namespace Wtq.Utils;

public static class EnumExtensions
{
	public static TAttr? GetAttribute<TEnum, TAttr>(this TEnum enumValue)
		where TEnum : struct
		where TAttr : Attribute
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

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
}