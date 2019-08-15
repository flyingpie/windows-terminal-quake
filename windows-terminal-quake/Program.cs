using System;
using System.Windows.Forms;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake
{
    public class Program
    {
        private static Toggler _toggler;
        private static TrayIcon _trayIcon;

        public static void Main(string[] args)
        {
            _trayIcon = new TrayIcon((s, a) => Close());

            try
            {
                _toggler = new Toggler();

                _trayIcon.Notify(ToolTipIcon.Info, $"Windows Terminal Quake is running, press CTRL+~ or CTRL+Q to toggle.");
            }
            catch (Exception ex)
            {
                _trayIcon.Notify(ToolTipIcon.Error, $"Cannot start: '{ex.Message}'.");

                Close();
            }
        }

        private static void Close()
        {
            _toggler?.Dispose();
            _toggler = null;

            _trayIcon?.Dispose();
            _trayIcon = null;
        }
    }
}