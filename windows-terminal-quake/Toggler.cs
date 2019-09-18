using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
    public class Toggler : IDisposable
    {
        private Process _process;
      
        public Toggler(Process process)
        {
            _process = process;

            // Hide from taskbar
            User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

            User32.Rect rect = default;
            var ok = User32.GetWindowRect(_process.MainWindowHandle, ref rect);
            var isOpen = rect.Top >= GetScreenWithCursor().Bounds.Y;
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);

            var stepCount = 10;

            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
            HotKeyManager.RegisterHotKey(Keys.Q, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += (s, a) =>
            {
                if (isOpen)
                {
                    isOpen = false;
                    Console.WriteLine("Close");

                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                    User32.SetForegroundWindow(_process.MainWindowHandle);

                    var bounds = GetScreenWithCursor().Bounds;

                    for (int i = stepCount - 1; i >= 0; i--)
                    {
                        User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                        Task.Delay(1).GetAwaiter().GetResult();
                    }

                    // Minimize, so the last window gets focus
                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);

                    // Hide, so the terminal windows doesn't linger on the desktop
                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
                }
                else
                {
                    isOpen = true;
                    Console.WriteLine("Open");

                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
                    User32.SetForegroundWindow(_process.MainWindowHandle);

                    var bounds = GetScreenWithCursor().Bounds;

                    for (int i = 1; i <= stepCount; i++)
                    {
                        User32.MoveWindow(_process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                        Task.Delay(1).GetAwaiter().GetResult();
                    }
                    User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);
                }
            };
        }

        public void Dispose()
        {
            ResetTerminal(_process);
        }

        private static Screen GetScreenWithCursor()
        {
            return Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
        }

        private static void ResetTerminal(Process process)
        {
            var bounds = GetScreenWithCursor().Bounds;

            // Restore taskbar icon
            User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

            // Reset position
            User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);

            // Restore window
            User32.ShowWindow(process.MainWindowHandle, NCmdShow.MAXIMIZE);
        }
    }
}