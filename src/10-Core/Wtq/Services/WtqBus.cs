namespace Wtq.Services;

/// <inheritdoc cref="IWtqBus"/>
public class WtqBus : IWtqBus
{
	private readonly ILogger _log = Log.For<WtqBus>();
	private readonly List<EventRegistration> _registrations = [];

	/// <inheritdoc/>
	public void OnEvent<TEvent>(
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent
	{
		Guard.Against.Null(onEvent);

		OnInternal(ev => ev is TEvent, ev => onEvent((TEvent)ev));
	}

	/// <inheritdoc/>
	public void OnEvent<TEvent>(
		Func<TEvent, bool> predicate,
		Func<TEvent, Task> onEvent)
		where TEvent : WtqEvent
	{
		Guard.Against.Null(predicate);
		Guard.Against.Null(onEvent);

		OnInternal(ev => ev is TEvent e && predicate(e), ev => onEvent((TEvent)ev));
	}

	/// <inheritdoc/>
	public void Publish<TEvent>()
		where TEvent : WtqEvent, new()
	{
		Publish(new TEvent());
	}

	/// <inheritdoc/>
	public void Publish(
		WtqEvent eventType)
	{
		Guard.Against.Null(eventType);

		_log.LogDebug("Publishing event '[{Type}] {Event}'", eventType.GetType().FullName, eventType);

		foreach (var reg in _registrations.Where(r => r.Predicate(eventType)))
		{
			_ = Task.Run(
				async () =>
				{
					try
					{
						await reg.OnEvent(eventType).NoCtx();
					}
					catch (Exception ex)
					{
						_log.LogWarning(ex, "Error publishing event '{Event}': {Message}", eventType, ex.Message);
					}
				});
		}
	}

	private void OnInternal(
		Func<WtqEvent, bool> predicate,
		Func<WtqEvent, Task> onEvent)
	{
		Guard.Against.Null(predicate);
		Guard.Against.Null(onEvent);

		_registrations.Add(new()
		{
			OnEvent = onEvent,
			Predicate = predicate,
		});
	}

	private sealed class EventRegistration
	{
		public required Func<WtqEvent, Task> OnEvent { get; init; }

		public required Func<WtqEvent, bool> Predicate { get; init; }
	}
}