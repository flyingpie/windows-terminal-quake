using System.Reflection;
using System.Text.RegularExpressions;

namespace Wtq.Services;

/// <summary>
/// Service that listens for events and sends them to any configured event hooks.
/// </summary>
public class WtqEventHookHandler(
	IOptionsMonitor<WtqOptions> opts,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: WtqHostedService
{
	private readonly IOptionsMonitor<WtqOptions> _opts = Guard.Against.Null(opts);
	private readonly IWtqAppRepo _appRepo = Guard.Against.Null(appRepo);
	private readonly IWtqBus _bus = Guard.Against.Null(bus);

	protected override Task OnStartAsync(CancellationToken cancellationToken)
	{
		// Listen to all event types.
		_bus.OnEvent<WtqEvent>(async ev =>
		{
			// Per-app event hooks.
			if (ev is WtqAppEvent appEvent)
			{
				// See if we can find the app this event is about.
				var app = _appRepo.GetByName(appEvent.AppName);

				// The app can sometimes be null, if we're in the process of reloading configuration.
				if (app != null)
				{
					await ExecuteEventHooksAsync(app.Options.EventHooks, ev);
				}
			}

			// Global event hooks (i.e. ones not necessarily related to an individual app).
			await ExecuteEventHooksAsync(_opts.CurrentValue.EventHooks, ev);
		});

		return Task.CompletedTask;
	}

	/// <summary>
	/// Takes a set of event hooks, and fires the ones that match the specified event.
	/// </summary>
	private static async Task ExecuteEventHooksAsync(ICollection<WtqAppEventHookOptions> hooks, WtqEvent ev)
	{
		Guard.Against.Null(hooks);
		Guard.Against.Null(ev);

		// Event name
		var eventName = ev
				.GetType()
				.GetCustomAttribute<DisplayNameAttribute>()
				?.DisplayName
			?? ev.GetType().Name;

		foreach (var hook in hooks)
		{
			var r = new Regex(hook.EventPattern, RegexOptions.IgnoreCase);

			if (!r.IsMatch(eventName))
			{
				continue;
			}

			// Setup informational properties to send to the executed program.
			var envVars = ev
				.GetType()
				.GetProperties()
				.ToDictionary(p => p.Name, p => p.GetValue(ev));

			// Also include the name of the event as a property.
			envVars["EventName"] = eventName;

			await hook.ExecuteAsync(envVars).NoCtx();

			// Stop execution if we've found a matching event hook.
			break;
		}
	}
}