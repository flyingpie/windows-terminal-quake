namespace Wtq.Core.Services;

public interface IWtqBus
{
	void On(Func<IWtqEvent, bool> predicate, Func<IWtqEvent, Task> onEvent);

	void On<TEvent>(Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent;

	void Publish(IWtqEvent eventType);
}