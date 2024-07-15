using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services.WinForms.Native;
using Wtq.Utils;

namespace Wtq.Services.WinForms;

public class WinFormsHotKeyService : IHostedService
{
	private readonly ILogger _log = Log.For<WinFormsHotKeyService>();
	private readonly IWtqBus _bus;

	public WinFormsHotKeyService(IWtqBus bus)
	{
		_bus = bus ?? throw new ArgumentNullException(nameof(bus));

		_bus.OnEvent<WtqRegisterHotKeyEvent>(
			e =>
			{
				var mods = (KeyModifiers)e.Modifiers;
				var key = (Keys)e.Key;

				_log.LogInformation("Registering HotKey [{Modifiers}] '{Key}'", mods, key);

				HotKeyManager.RegisterHotKey(key, mods);

				return Task.CompletedTask;
			});

		HotKeyManager.HotKeyPressed += (s, a) =>
		{
			_bus.Publish(new WtqHotKeyPressedEvent()
			{
				Key = a.Key.ToWtqKeys(),
				Modifiers = a.Modifiers.ToWtqKeyModifiers(),
			});
		};
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}