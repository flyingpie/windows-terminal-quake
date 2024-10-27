namespace Wtq.Services.KWin.Scripting;

public static class JsUtils
{
	[SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "MvdO: Needs to be lower-case for JS runtime.")]
	public static string ToJsBoolean(bool b)
	{
		return b.ToString().ToLowerInvariant();
	}
}