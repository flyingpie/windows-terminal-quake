using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading;
using Wtq.Configuration;
using Wtq.Core.Data;
using Wtq.Core.Events;

//using Wtq.Core.Service;
using Wtq.Core.Services;
using Wtq.Services;

namespace Wtq;

public sealed class WtqService(
	ILogger<WtqService> log,
	IOptions<WtqOptions> opts,
	WtqAppMonitorService appMon,
	//IWtqHotkeyService hkService,
	IWtqAppToggleService toggler,
	IWtqAppRepo appRepo,
	IWtqBus bus)
	: IHostedService
{
	private readonly ILogger<WtqService> _log = log ?? throw new ArgumentNullException(nameof(log));
	private readonly IOptions<WtqOptions> _opts = opts ?? throw new ArgumentNullException(nameof(opts));
	//private readonly IWtqHotkeyService _hkService = hkService ?? throw new ArgumentNullException(nameof(hkService));

	private readonly WtqAppMonitorService _appMon = appMon ?? throw new ArgumentNullException(nameof(appMon));
	private readonly IWtqAppToggleService _toggler = toggler ?? throw new ArgumentNullException(nameof(toggler));
	private readonly IWtqAppRepo _appRepo = appRepo ?? throw new ArgumentNullException(nameof(appRepo));
	private readonly IWtqBus _bus = bus ?? throw new ArgumentNullException(nameof(bus));

	//private Plexiglass _gl;

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Starting");

		//_gl = new Plexiglass();

		//HotkeyManager.HotKeyPressed += async (s, args) => await ToggleStuffAsync(args);
		//_hkService.OnHotkey(ToggleStuffAsync);
		_bus.OnAsync(e => e is WtqToggleAppEvent, ToggleStuffAsync);

		_bus.OnAsync(
			e => e is WtqAppFocusEvent,
			async e =>
			{
				var ev = (WtqAppFocusEvent)e;

				if(ev.App != null && ev.App == open && !ev.GainedFocus)
				{
					await open.CloseAsync();
					open = null;
				}

			});

		// Global hotkeys.
		foreach (var hk in _opts.Value.Hotkeys)
		{
			_log.LogInformation("Registering global hotkey '{Hotkey}'", hk);
			//HotkeyManager.RegisterHotKey(hk.Key, hk.Modifiers);
		}

		// Per-app hotkeys.
		foreach (var app in _opts.Value.Apps)
		{
			foreach (var hk in app.Hotkeys)
			{
				_log.LogInformation("Registering hotkey '{Hotkey}' for app '{App}'", hk, app);
				//HotkeyManager.RegisterHotKey(hk.Key, hk.Modifiers);
			}
		}

		return Task.CompletedTask;
	}

	private WtqApp? open = null;
	private WtqApp? _lastOpen = null;

	private async Task ToggleStuffAsync(IWtqEvent ev)
	{
		const int openTimeMs = 100;
		const int switchTimeMs = 50;

		//_log.LogInformation("Pressed hot key ['{Modifiers}'] + '{HotKey}'", args.Modifiers, args.Key);

		//var app = _opts.Value.Apps.FirstOrDefault(a => a.HasHotkey(args.Key, args.Modifiers));
		//var app = _opts.Value.Apps.FirstOrDefault(a => a.);

		var wasOpen = open != null;

		var app = ev.App;

		// If the action does not point to a single app, toggle the most recent one.
		if (app == null)
		{
			if (open != null)
			{
				//await _toggler.ToggleAsync(open.Process, false, openTimeMs);
				await open.CloseAsync();
				_lastOpen = open;
				open = null;
				_appMon.DropFocus();
				return;
			}
			else
			{
				if (_lastOpen == null)
				{
					// TODO
					var first = _appRepo.Apps.FirstOrDefault();
					if (first != null)
					{
						await first.OpenAsync();
					}
					open = first;
					_lastOpen = first;
					return;
				}

				open = _lastOpen;
				//await _toggler.ToggleAsync(open.Process, true, openTimeMs);
				await open.OpenAsync();
				return;
			}

			//_log.LogWarning("No app found with assigned hotkeys ['{Modifiers}'] + '{HotKey}'", args.Modifiers, args.Key);
			return;
		}

		//var process = _appMon.GetProcessForApp(ev.App);

		//if (!ev.App.IsActive)
		//{
		//	_log.LogWarning("No process found for app '{App}'", ev.App);
		//	return;
		//}

		// We can't toggle apps that are not active.
		if (!app.IsActive)
		{
			_log.LogWarning("WTQ process for app '{App}' does not have a process instance assigned", app);
			return;
		}

		if (open != null)
		{
			if (open == app)
			{
				await app.CloseAsync();
				//await _toggler.ToggleAsync(open.Process, false, openTimeMs);
				_lastOpen = open;
				open = null;
				_appMon.DropFocus();
			}
			else
			{
				//await _toggler.ToggleAsync(open.Process, false, switchTimeMs);
				//await _toggler.ToggleAsync(process.Process, true, switchTimeMs);
				await open.CloseAsync(ToggleModifiers.SwitchingApps);
				await app.OpenAsync(ToggleModifiers.SwitchingApps);

				open = app;
			}

			return;
		}

		_log.LogInformation("Toggling app {App}", app);
		await app.OpenAsync();

		open = app;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_log.LogInformation("Stopping");

		return Task.CompletedTask;
	}
}

//internal class Plexiglass : Form
//{
//	public Plexiglass()
//	{
//		this.BackColor = Color.DarkGray;
//		this.Opacity = 0.30;      // Tweak as desired
//		this.FormBorderStyle = FormBorderStyle.None;
//		this.ControlBox = false;
//		this.ShowInTaskbar = false;
//		this.StartPosition = FormStartPosition.Manual;
//		this.AutoScaleMode = AutoScaleMode.None;
//		//this.Location = tocover.PointToScreen(Point.Empty);
//		//this.ClientSize = tocover.ClientSize;
//		this.Location = new Point(100, 800);
//		this.Size = new Size(3000, 200);
//		//tocover.LocationChanged += Cover_LocationChanged;
//		//tocover.ClientSizeChanged += Cover_ClientSizeChanged;
//		this.Show();
//		//		tocover.Focus();
//		// Disable Aero transitions, the plexiglass gets too visible
//		if (Environment.OSVersion.Version.Major >= 6)
//		{
//			int value = 1;
//			//			DwmSetWindowAttribute(tocover.Handle, DWMWA_TRANSITIONS_FORCEDISABLED, ref value, 4);
//		}
//	}

//	//private void Cover_LocationChanged(object sender, EventArgs e)
//	//{
//	//	// Ensure the plexiglass follows the owner
//	//	this.Location = this.Owner.PointToScreen(Point.Empty);
//	//}
//	//private void Cover_ClientSizeChanged(object sender, EventArgs e)
//	//{
//	//	// Ensure the plexiglass keeps the owner covered
//	//	this.ClientSize = this.Owner.ClientSize;
//	//}
//	//protected override void OnFormClosing(FormClosingEventArgs e)
//	//{
//	//	// Restore owner
//	//	this.Owner.LocationChanged -= Cover_LocationChanged;
//	//	this.Owner.ClientSizeChanged -= Cover_ClientSizeChanged;
//	//	if (!this.Owner.IsDisposed && Environment.OSVersion.Version.Major >= 6)
//	//	{
//	//		int value = 1;
//	//		DwmSetWindowAttribute(this.Owner.Handle, DWMWA_TRANSITIONS_FORCEDISABLED, ref value, 4);
//	//	}
//	//	base.OnFormClosing(e);
//	//}
//	protected override void OnActivated(EventArgs e)
//	{
//		// Always keep the owner activated instead
//		this.BeginInvoke(new Action(() => this.Owner.Activate()));
//	}

//	private const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

//	[DllImport("dwmapi.dll")]
//	private static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int value, int attrLen);
//}