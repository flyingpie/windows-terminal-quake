using System.Reflection;

namespace Wtq.Utils;

public static class OptionUtils
{
	public static TValue Cascade<TValue>(
		Expression<Func<WtqSharedOptions, object?>> expr,
		params WtqSharedOptions[] opts
	)
	{
		Guard.Against.Null(expr);

		var v = expr.Compile();

		foreach (var opt in opts)
		{
			var fromApp = v(opt);
			if (fromApp != null)
			{
				return (TValue)fromApp;
			}
		}

		return AttrUtils.GetDefaultValueFor<TValue>(expr);
	}
}