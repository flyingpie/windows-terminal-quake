namespace Wtq.Events;

/// <summary>
/// Base class for all events, used for generics and constraints.
/// </summary>
public abstract class WtqEvent
{
	public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;
}