#pragma warning disable // MvdO: Hacky hook into the WinForms hotkey system.

namespace Wtq.Services.WinForms.Native;

/// <summary>
/// Wrapper around Windows Forms global hotkey functionality. Surfaces a delegate to handle the hotkey being pressed.
/// </summary>
internal static class HotkeyManager
{
	private static readonly ManualResetEvent _windowReadyEvent = new(false);

	private static volatile nint _hwnd;

	private static int _id;

	private static volatile MessageWindow _wnd;

	static HotkeyManager()
	{
		Thread messageLoop = new(delegate ()
		{
			Application.Run(new MessageWindow());
		})
		{
			Name = $"{nameof(Wtq)}.{nameof(WinForms)}.{nameof(HotkeyManager)}",
			IsBackground = true,
		};

		messageLoop.Start();
	}

	private delegate void RegisterHotkeyDelegate(nint hwnd, int id, uint modifiers, uint key);

	private delegate void UnregisterHotkeyDelegate(nint hwnd, int id);

	/// <summary>
	/// Fired when the registered hotkey is pressed. Note that <see cref="RegisterHotkey"/> needs to be called first.
	/// </summary>
	public static event EventHandler<HotkeyEventArgs> HotkeyPressed = (sender, e) => { };

	public static void Exit()
	{
		Application.Exit();
	}

	public static int RegisterHotkey(Keys key, KeyModifiers modifiers)
	{
		_windowReadyEvent.WaitOne();
		int id = Interlocked.Increment(ref _id);
		_wnd.Invoke(new RegisterHotkeyDelegate((hwnd, id, modifiers, key) => User32.RegisterHotKey(hwnd, id, modifiers, key)), _hwnd, id, (uint)modifiers, (uint)key);

		return id;
	}

	public static void UnregisterHotkey(int id)
	{
		_wnd.Invoke(new UnregisterHotkeyDelegate((hwnd, id) => User32.UnregisterHotKey(_hwnd, id)), _hwnd, id);
	}

	/// <summary>
	/// We need a window to catch the global hotkey event.
	/// </summary>
	private sealed class MessageWindow : Form
	{
		private const int WMHotkey = 0x312;

		public MessageWindow()
		{
			_wnd = this;
			_hwnd = Handle;
			_windowReadyEvent.Set();
		}

		protected override void SetVisibleCore(bool value)
		{
			// Ensure the window never becomes visible
			base.SetVisibleCore(false);
		}

		protected override void WndProc(ref Message m)
		{
			Console.WriteLine($"WndProc({m.Msg})");

			if (m.Msg == WMHotkey)
			{
				HotkeyEventArgs e = new(m.LParam);
				HotkeyPressed?.Invoke(null, e);
			}

			base.WndProc(ref m);
		}
	}
}