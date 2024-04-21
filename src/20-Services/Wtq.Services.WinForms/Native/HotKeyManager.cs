using Wtq.WinForms.Native;

namespace Wtq.Services.WinForms.Native;

/// <summary>
/// Wrapper around Windows Forms global hot key functionality. Surfaces a delegate to handle the hot key being pressed.
/// </summary>
internal static class HotKeyManager
{
	private static readonly ManualResetEvent _windowReadyEvent = new(false);

	private static volatile nint _hwnd;

	private static int _id;

	private static volatile MessageWindow _wnd;

	static HotKeyManager()
	{
		Thread messageLoop = new(delegate ()
		{
			Application.Run(new MessageWindow());
		})
		{
			Name = $"{nameof(Wtq)}.{nameof(WinForms)}.{nameof(HotKeyManager)}",
			IsBackground = true,
		};

		messageLoop.Start();
	}

	private delegate void RegisterHotKeyDelegate(nint hwnd, int id, uint modifiers, uint key);

	private delegate void UnRegisterHotKeyDelegate(nint hwnd, int id);

	/// <summary>
	/// Fired when the registered hot key is pressed. Note that <see cref="RegisterHotKey"/> needs to be called first.
	/// </summary>
	public static event EventHandler<HotKeyEventArgs> HotKeyPressed = (sender, e) => { };

	public static int RegisterHotKey(Keys key, KeyModifiers modifiers)
	{
		_windowReadyEvent.WaitOne();
		int id = Interlocked.Increment(ref _id);
		_wnd.Invoke(new RegisterHotKeyDelegate((hwnd, id, modifiers, key) => User32.RegisterHotKey(hwnd, id, modifiers, key)), _hwnd, id, (uint)modifiers, (uint)key);

		return id;
	}

	public static void UnregisterHotKey(int id)
	{
		_wnd.Invoke(new UnRegisterHotKeyDelegate((hwnd, id) => User32.UnregisterHotKey(_hwnd, id)), _hwnd, id);
	}

	/// <summary>
	/// We need a window to catch the global hot key event.
	/// </summary>
	private sealed class MessageWindow : Form
	{
		private const int WMHotKey = 0x312;

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
			if (m.Msg == WMHotKey)
			{
				HotKeyEventArgs e = new(m.LParam);
				HotKeyPressed?.Invoke(null, e);
			}

			base.WndProc(ref m);
		}
	}
}