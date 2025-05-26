namespace Wtq.Events;

[DisplayName("AppToggledOn")]
public class WtqAppToggledOnEvent(string appName, bool isSwitching)
	: WtqAppEvent(appName)
{
	public bool IsSwitching { get; set; } = isSwitching;
}