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
				// Event name
				var n = ev.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? ev.GetType().Name;

				var p = ev
					.GetType()
					.GetProperties()
					.ToDictionary(p => p.Name, p => p.GetValue(ev));

				p["EventName"] = n;

				// Per-app event hooks
				if (ev is WtqAppEvent appEvent)
				{
					var app = _appRepo.GetByName(appEvent.AppName);

					if (app?.Options?.EventHooks?.TryGetValue(n, out var appHook) ?? false)
					{
						await appHook.ExecuteAsync(p).NoCtx();
						return;
					}
				}

				// Global event hooks
				if (_opts.CurrentValue.EventHooks.TryGetValue(n, out var globalHook))
				{
					await globalHook.ExecuteAsync(p).NoCtx();
				}

				foreach (var hook in _opts.CurrentValue.EventHooks)
				{
					var r = new Regex(hook.Key, RegexOptions.IgnoreCase);

					if (r.IsMatch(n))
					{
						hook.Value.ExecuteAsync(p).NoCtx();
					}
				}
			});

		return Task.CompletedTask;
	}
}