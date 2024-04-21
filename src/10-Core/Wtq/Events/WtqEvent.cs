namespace Wtq.Events;

public class WtqEvent : IWtqEvent
{
	public WtqApp? App { get; set; }

	public override string ToString() => $"[{nameof(WtqEvent)}] App:{App}";
}