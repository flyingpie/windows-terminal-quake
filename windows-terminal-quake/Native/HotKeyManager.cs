using System;
using System.Threading;
using System.Windows.Forms;

namespace WindowsTerminalQuake.Native
{
	/// <summary>
	/// Wrapper around Windows Forms global hot key functionality. Surfaces an delegate to handle the hot key being pressed.
	/// </summary>
	public static class HotKeyManager
	{
		/// <summary>
		/// Fired when the registered hot key is pressed. Note that <see cref="RegisterHotKey"/> needs to be called first.
		/// </summary>
		public static event EventHandler<HotKeyEventArgs> HotKeyPressed;

		private delegate void RegisterHotKeyDelegate(IntPtr hwnd, int id, uint modifiers, uint key);

		private delegate void UnRegisterHotKeyDelegate(IntPtr hwnd, int id);

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

		private static volatile MessageWindow _wnd;
		private static volatile IntPtr _hwnd;
		private static ManualResetEvent _windowReadyEvent = new ManualResetEvent(false);

		static HotKeyManager()
		{
			Thread messageLoop = new Thread(delegate ()
			{
				Application.Run(new MessageWindow());
			});
			messageLoop.Name = "MessageLoopThread";
			messageLoop.IsBackground = true;
			messageLoop.Start();
		}

		/// <summary>
		/// We need a window to catch the global hot key event.
		/// </summary>
		private class MessageWindow : Form
		{
			public MessageWindow()
			{
				_wnd = this;
				_hwnd = Handle;
				_windowReadyEvent.Set();
			}

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == WM_HOTKEY)
				{
					HotKeyEventArgs e = new HotKeyEventArgs(m.LParam);
					HotKeyPressed?.Invoke(null, e);
				}

				base.WndProc(ref m);
			}

			protected override void SetVisibleCore(bool value)
			{
				// Ensure the window never becomes visible
				base.SetVisibleCore(false);
			}

			private const int WM_HOTKEY = 0x312;
		}

		private static int _id = 0;
	}

	public class HotKeyEventArgs : EventArgs
	{
		public readonly Keys Key;
		public readonly KeyModifiers Modifiers;

		public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
		{
			Key = key;
			Modifiers = modifiers;
		}

		public HotKeyEventArgs(IntPtr hotKeyParam)
		{
			uint param = (uint)hotKeyParam.ToInt64();
			Key = (Keys)((param & 0xffff0000) >> 16);
			Modifiers = (KeyModifiers)(param & 0x0000ffff);
		}
	}
}