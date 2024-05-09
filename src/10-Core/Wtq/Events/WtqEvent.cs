namespace Wtq.Events;

public class WtqEvent : IWtqEvent
{
	public WtqApp? App { get; init; }

	public override string ToString() => $"[{nameof(WtqEvent)}] App:{App}";
}