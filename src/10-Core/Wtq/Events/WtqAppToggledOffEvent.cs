namespace Wtq.Events;

[DisplayName("AppToggledOff")]
public class WtqAppToggledOffEvent(string appName, bool isSwitching) : WtqAppEvent(appName)
{
	public bool IsSwitching { get; set; } = isSwitching;
}