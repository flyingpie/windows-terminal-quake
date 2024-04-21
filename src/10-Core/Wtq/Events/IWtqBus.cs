namespace Wtq.Events;

public interface IWtqBus
{
	void On<TEvent>(Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent;

	void Publish(IWtqEvent eventType);
}