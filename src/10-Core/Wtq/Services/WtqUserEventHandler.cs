using System.Reflection;

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

	protected override async Task OnStartAsync(CancellationToken cancellationToken)
	{
		_bus.OnEvent<WtqEvent>(
			async ev =>
			{
				var n = ev.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? ev.GetType().Name;
				Console.WriteLine($"EVENT:{n}");

				// Per-app event hooks
				if (ev is WtqAppEvent appEvent)
				{
					var app = _appRepo.GetByName(appEvent.AppName);

					if (app?.Options?.EventHooks?.TryGetValue(n, out var val) ?? false)
					{
						await val.ExecuteAsync().NoCtx();
						return;
					}
				}

				// Global event hooks
				if (_opts.CurrentValue.EventHooks.TryGetValue(n, out var val1))
				{
					await val1.ExecuteAsync().NoCtx();
				}
			});
	}
}