namespace Wtq.Core.Services;

public interface IWtqBus
{
	void OnAsync(Func<IWtqEvent, bool> predicate, Func<IWtqEvent, Task> onEvent);

	void Publish(IWtqEvent eventType);
}