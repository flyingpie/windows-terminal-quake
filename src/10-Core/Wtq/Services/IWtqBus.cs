namespace Wtq.Services;

public interface IWtqBus
{
	void OnEvent<TEvent>(
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent;

	void OnEvent<TEvent>(
		Func<TEvent, bool> predicate,
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent;

	void Publish(
		WtqEvent eventType);
}