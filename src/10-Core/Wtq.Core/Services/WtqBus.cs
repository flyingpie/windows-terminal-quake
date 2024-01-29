namespace Wtq.Core.Services;

public class WtqBus : IWtqBus
{
	private readonly ILogger _log = Log.For<WtqBus>();
	private readonly List<EventRegistration> _registrations = [];

	public void OnAsync(Func<IWtqEvent, bool> predicate, Func<IWtqEvent, Task> onEvent)
	{
		_registrations.Add(new EventRegistration()
		{
			OnEvent = onEvent,
			Predicate = predicate,
		});
	}

	public void Publish(IWtqEvent ev)
	{
		_log.LogInformation("Publishing event '[{Type}] {Event}'", ev.GetType().FullName, ev);

		foreach (var reg in _registrations.Where(r => r.Predicate(ev)))
		{
			_ = Task.Run(async () =>
			{
				try
				{
					await reg.OnEvent(ev);
				}
				catch (Exception ex)
				{
					_log.LogWarning(ex, "Error publishing event '{Event}': {Message}", ev, ex.Message);
				}
			});
		}
	}

	private sealed class EventRegistration
	{
		public Func<IWtqEvent, bool> Predicate { get; init; }

		public Func<IWtqEvent, Task> OnEvent { get; init; }
	}
}