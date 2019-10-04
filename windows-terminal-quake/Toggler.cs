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
        private static int offsetLeft = 0;
        private static int width = 0;
        private static int height = 0;

        private static readonly int stepCount = 10;

        private Process _process;
        private Config _config;

        public Toggler(Process process, Config config)
        {
            _process = process;
            _config = config;

            // Hide from taskbar
            User32.SetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(_process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW);

            if (config.Maximize)
            {
                User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MAXIMIZE);
            }

            User32.Rect rect = default;
            User32.GetWindowRect(_process.MainWindowHandle, ref rect);
            var terminalWindow = new TerminalWindow(rect, GetScreenWithCursor().Bounds);
            if (config.Center)
            {
                terminalWindow.CenterHorizontally();
            }
            terminalWindow.EnsureVisible();
            SaveTerminalState(terminalWindow);

            User32.MoveWindow(_process.MainWindowHandle, terminalWindow.ScreenX, terminalWindow.ScreenY, terminalWindow.Width, terminalWindow.Height, true);
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.SHOW);
            User32.SetForegroundWindow(_process.MainWindowHandle);

            HotKeyManager.RegisterHotKey(Keys.Oemtilde, KeyModifiers.Control);
            HotKeyManager.RegisterHotKey(Keys.Q, KeyModifiers.Control);

            HotKeyManager.HotKeyPressed += handleHotKeyPressed;
        }

        private void handleHotKeyPressed(object e, HotKeyEventArgs a)
        {
            User32.Rect rect = default;
            User32.GetWindowRect(_process.MainWindowHandle, ref rect);
            var terminalWindow = new TerminalWindow(rect, GetScreenWithCursor().Bounds);

            if (terminalWindow.IsVisible() && terminalWindow.IsDocked())
            {
                HideTerminalWithEffects(terminalWindow);
            }
            else
            {
                ShowTerminalWithEffects(terminalWindow);
            }
        }

        private void HideTerminalWithEffects(TerminalWindow terminalWindow)
        {
            SaveTerminalState(terminalWindow);

            if (terminalWindow.Top > 0)
            {
                HideProcessWindow();
                return;
            }

            Console.WriteLine("Close");

            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.SHOW);
            User32.SetForegroundWindow(_process.MainWindowHandle);

            var stepSize = (double)height / (double)stepCount;
            for (int i = 1; i <= stepCount; i++)
            {

                User32.MoveWindow(
                    _process.MainWindowHandle, 
                    terminalWindow.ScreenX, 
                    terminalWindow.ScreenY - (int)Math.Round(stepSize * i), 
                    terminalWindow.Width, 
                    terminalWindow.Height, 
                    true
                );

                Task.Delay(1).GetAwaiter().GetResult();
            }

            HideProcessWindow();
        }

        private void ShowTerminalWithEffects(TerminalWindow terminalWindow)
        {
            Console.WriteLine("Open");
            terminalWindow.Left = offsetLeft;
            terminalWindow.Width = width;
            terminalWindow.Top = -height;
            terminalWindow.Height = height;
            terminalWindow.EnsureWillFitOnScreen();

            if (_config.Center)
            {
                terminalWindow.CenterHorizontally();
            }

            User32.MoveWindow(
                _process.MainWindowHandle,
                terminalWindow.ScreenX,
                terminalWindow.ScreenY - terminalWindow.Height,
                terminalWindow.Width,
                terminalWindow.Height,
                false
            );

            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.RESTORE);
            User32.SetForegroundWindow(_process.MainWindowHandle);

            var stepSize = (double)height / (double)stepCount;
            for (int i = 1; i <= stepCount; i++)
            {
                User32.MoveWindow(
                    _process.MainWindowHandle,
                    terminalWindow.ScreenX,
                    terminalWindow.ScreenY + (int)Math.Round(stepSize * i),
                    terminalWindow.Width,
                    terminalWindow.Height,
                    true
                );

                Task.Delay(1).GetAwaiter().GetResult();
            }
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.SHOW);
        }

        private void HideProcessWindow() { 
            // Minimize, so the last window gets focus
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.MINIMIZE);

            // Hide, so the terminal windows doesn't linger on the desktop
            User32.ShowWindow(_process.MainWindowHandle, NCmdShow.HIDE);
        }

        private void SaveTerminalState(TerminalWindow terminalWindow)
        {
            width = terminalWindow.Width;
            height = terminalWindow.Height;
            offsetLeft = terminalWindow.Left;
        }

        private static Screen GetScreenWithCursor()
        {
            return Screen.AllScreens.FirstOrDefault(s => s.Bounds.Contains(Cursor.Position));
        }

        public void Dispose()
        {
            ResetTerminal(_process);
        }
        private static void ResetTerminal(Process process)
        {
            User32.Rect rect = default;
            User32.GetWindowRect(process.MainWindowHandle, ref rect);

            var terminalWindow = new TerminalWindow(rect, GetScreenWithCursor().Bounds);

            // Restore taskbar icon
            User32.SetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE, (User32.GetWindowLong(process.MainWindowHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & User32.WS_EX_APPWINDOW);

            // Reset position
            User32.MoveWindow(process.MainWindowHandle, terminalWindow.ScreenX, terminalWindow.ScreenY, terminalWindow.Width, terminalWindow.Height, true);

            // Restore window
            User32.ShowWindow(process.MainWindowHandle, NCmdShow.SHOW);
        }
    }
}