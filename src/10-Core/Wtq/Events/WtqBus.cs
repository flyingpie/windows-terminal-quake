namespace Wtq.Events;

public class WtqBus : IWtqBus
{
	private readonly ILogger _log = Log.For<WtqBus>();
	private readonly List<EventRegistration> _registrations = [];

	public void On(Func<IWtqEvent, bool> predicate, Func<IWtqEvent, Task> onEvent)
	{
		Guard.Against.Null(predicate);
		Guard.Against.Null(onEvent);

		_registrations.Add(new EventRegistration()
		{
			OnEvent = onEvent,
			Predicate = predicate,
		});
	}

	public void OnEvent<TEvent>(Func<TEvent, Task> onEvent)
		where TEvent : IWtqEvent
	{
		Guard.Against.Null(onEvent);

		On(ev => ev is TEvent, ev => onEvent((TEvent)ev));
	}

	public void Publish(IWtqEvent eventType)
	{
		Guard.Against.Null(eventType);

		_log.LogInformation("Publishing event '[{Type}] {Event}'", eventType.GetType().FullName, eventType);

		foreach (var reg in _registrations.Where(r => r.Predicate(eventType)))
		{
			_ = Task.Run(async () =>
			{
				try
				{
					await reg.OnEvent(eventType).ConfigureAwait(false);
				}
				catch (Exception ex)
				{
					_log.LogWarning(ex, "Error publishing event '{Event}': {Message}", eventType, ex.Message);
				}
			});
		}
	}

	private sealed class EventRegistration
	{
		public required Func<IWtqEvent, Task> OnEvent { get; init; }

		public required Func<IWtqEvent, bool> Predicate { get; init; }
	}
}