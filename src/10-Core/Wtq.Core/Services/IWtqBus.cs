namespace Wtq.Core.Services;

public interface IWtqBus
{
	void On(Func<IWtqEvent, bool> predicate, Func<IWtqEvent, Task> onEvent);

	void Publish(IWtqEvent eventType);
}