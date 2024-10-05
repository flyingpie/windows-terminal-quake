namespace Wtq.Events;

public interface IWtqBus
{
	void OnEvent<TEvent>(Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent;

	void Publish(IWtqEvent eventType);
}