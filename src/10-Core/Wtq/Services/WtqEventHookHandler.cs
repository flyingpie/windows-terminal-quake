using System.Reflection;
using System.Text.RegularExpressions;

namespace Wtq.Services;

/// <summary>
/// 
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
		_bus.OnEvent<WtqEvent>(async ev =>
		{
			// Per-app event hooks
			if (ev is WtqAppEvent appEvent)
			{
				var app = _appRepo.GetByName(appEvent.AppName);

				if (app != null)
				{
					await ExecuteEventHooksAsync(app.Options.EventHooks, ev);
				}
			}

			// Global event hooks
			await ExecuteEventHooksAsync(_opts.CurrentValue.EventHooks, ev);
		});

		return Task.CompletedTask;
	}

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

			// Setup environment vars to send to the executed program.
			var envVars = ev
				.GetType()
				.GetProperties()
				.ToDictionary(p => p.Name, p => p.GetValue(ev));

			envVars["EventName"] = eventName;

			await hook.ExecuteAsync(envVars).NoCtx();

			break;
		}
	}
}