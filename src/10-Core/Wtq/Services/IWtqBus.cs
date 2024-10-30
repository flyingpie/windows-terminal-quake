namespace Wtq.Services;

/// <summary>
/// Handles subscribing to- and publishing events.
/// </summary>
public interface IWtqBus
{
	/// <summary>
	/// Register event handler for the specified event type.
	/// </summary>
	void OnEvent<TEvent>(
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent;

	/// <summary>
	/// Register event handler for the specified event type, and predicate.
	/// </summary>
	void OnEvent<TEvent>(
		Func<TEvent, bool> predicate,
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent;

	/// <summary>
	/// Publish an event.
	/// </summary>
	void Publish(
		WtqEvent eventType);
}