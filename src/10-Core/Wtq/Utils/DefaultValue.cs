using System.Reflection;

namespace Wtq.Utils;

public static class DefaultValue
{
	public static TValue For<TValue>(Expression expr)
	{
		var val = SystemExtensions.GetMemberInfo(expr).GetCustomAttribute<DefaultValueAttribute>()?.Value;
		if (val != null)
		{
			return (TValue)val;
		}

		return default;
	}
}