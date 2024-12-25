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
}