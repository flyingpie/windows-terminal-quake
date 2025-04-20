using System.Reflection;
using System.Text.RegularExpressions;

namespace Wtq.Services;

public class WtqUserEventHandler(
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
		_bus.OnEvent<WtqEvent>(
			async ev =>
			{

				// Per-app event hooks
				if (ev is WtqAppEvent appEvent)
				{
					var app = _appRepo.GetByName(appEvent.AppName);
					if (await ExecuteEventHooksAsync(app.Options.EventHooks, ev))
					{
						return;
					}
				}

				// Global event hooks
				await ExecuteEventHooksAsync(_opts.CurrentValue.EventHooks, ev);
			});

		return Task.CompletedTask;
	}

	private async Task<bool> ExecuteEventHooksAsync(WtqAppEventHooksOptions hooks, WtqEvent ev)
	{
		Guard.Against.Null(hooks);
		Guard.Against.Null(ev);

		// Event name
		var n = ev.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? ev.GetType().Name;

		var p = ev
			.GetType()
			.GetProperties()
			.ToDictionary(p => p.Name, p => p.GetValue(ev));

		p["EventName"] = n;

		foreach (var hook in hooks)
		{
			var r = new Regex(hook.Key, RegexOptions.IgnoreCase);

			if (r.IsMatch(n))
			{
				await hook.Value.ExecuteAsync(p).NoCtx();

				return hook.Value.StopPropagation;
			}
		}

		return false;
	}
}