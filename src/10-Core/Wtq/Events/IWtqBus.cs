namespace Wtq.Events;

public interface IWtqBus
{
	// void On(
	// 	Func<IWtqEvent, bool> predicate,
	// 	Func<IWtqEvent, Task> onEvent);

	void OnEvent<TEvent>(
		Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent;

	void OnEvent<TEvent>(
		Func<TEvent, bool> predicate,
		Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent;

	void Publish(
		IWtqEvent eventType);
}