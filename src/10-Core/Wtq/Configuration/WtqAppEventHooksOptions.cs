namespace Wtq.Configuration;

public class WtqAppEventHooksOptions
	: Dictionary<string, WtqAppEventHookOptions>
{
	public WtqAppEventHooksOptions()
		: base(StringComparer.OrdinalIgnoreCase)
	{
	}
}