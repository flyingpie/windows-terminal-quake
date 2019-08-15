using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsTerminalQuake.Native;

namespace WindowsTerminalQuake
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var process = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
            if (process == null)
            {
                Console.WriteLine("No WindowsTerminal process found");
                return;
            }

            // Hide from taskbar
            User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

            User32.Rect rect = default;
            var ok = User32.GetWindowRect(process.MainWindowHandle, ref rect);
            var isOpen = rect.Top >= GetScreenWithCursor().Bounds.Y;

            var stepCount = 10;

            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
            HotKeyManager.RegisterHotKey(Keys.Q, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += (s, a) =>
            {
                if (isOpen)
                {
                    isOpen = false;
                    Console.WriteLine("Close");

                    User32.ShowWindow(process.MainWindowHandle, NCmdShow.RESTORE);
                    User32.SetForegroundWindow(process.MainWindowHandle);

                    var bounds = GetScreenWithCursor().Bounds;

                    for (int i = stepCount - 1; i >= 0; i--)
                    {   
                        User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                        Task.Delay(1).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    isOpen = true;
                    Console.WriteLine("Open");

                    User32.ShowWindow(process.MainWindowHandle, NCmdShow.RESTORE);
                    User32.SetForegroundWindow(process.MainWindowHandle);

                    var bounds = GetScreenWithCursor().Bounds;

                    for (int i = 1; i <= stepCount; i++)
                    {
                        User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y + (-bounds.Height + (bounds.Height / stepCount * i)), bounds.Width, bounds.Height, true);

                        Task.Delay(1).GetAwaiter().GetResult();
                    }
                }
            };

            Console.WriteLine("Press any key to exit");
            Console.Read();

            ResetTerminal(process);
        }

        private static void ResetTerminal(Process process)
        {
            var bounds = GetScreenWithCursor().Bounds;

            // Restore taskbar icon
            User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

            // Reset position
            User32.MoveWindow(process.MainWindowHandle, bounds.X, bounds.Y, bounds.Width, bounds.Height, true);
        }

        public static Screen GetScreenWithCursor()
        {
            return Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
        }
    }
}