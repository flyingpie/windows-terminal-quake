namespace Wtq.Events;

[DisplayName("AppToggledOn")]
public class WtqAppToggledOnEvent : WtqAppEvent
{
	public bool IsSwitching { get; set; }
}