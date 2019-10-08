using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using WindowsTerminalQuake.Native;
using WindowsTerminalQuake.UI;

namespace WindowsTerminalQuake
{
    public class Program
    {
        private static Toggler _toggler;
        private static TrayIcon _trayIcon;

        public static void Main(string[] args)
        {
            // don't allow more than one instance to be running simultaneously
            var existingProcesses = Process.GetProcessesByName("windows-terminal-quake").Count();
            if (existingProcesses > 1)
            {
                return;
            }

            Application.ApplicationExit += (sender, e) =>
            {
                Close();
            };

            var config = new Config
            {
                Maximize = args.Contains("maximize"),
                Center = args.Contains("center")
            };

            _trayIcon = new TrayIcon((s, a) => Close());

            try
            {
                var processes = Process.GetProcessesByName("WindowsTerminal").Where(e => e.MainWindowHandle != default(IntPtr)).ToArray();
                Process process = null;
                int i = 0;
                while (i < processes.Length)
                {
                    try
                    {
                        process = processes[i];
                        bindProcessEvents(process);
                        break;
                    } catch (Exception)
                    {
                        i++;
                    }
                }

                if (process == null)
                {
                    process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "wt",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            WindowStyle = config.Maximize ? ProcessWindowStyle.Maximized : ProcessWindowStyle.Hidden,
                        }
                    };
                    process.Start();
                    while (process.MainWindowTitle == ""  || process.MainWindowTitle == "DesktopWindowXamlSource")
                    {
                        Thread.Sleep(10);
                        process.Refresh();
                    }
                    bindProcessEvents(process);
                }

                _toggler = new Toggler(process, config);

                _trayIcon.Notify(ToolTipIcon.Info, $"Windows Terminal Quake is running, press CTRL+~ or CTRL+Q to toggle.");
            }
            catch (Exception ex)
            {
                _trayIcon.Notify(ToolTipIcon.Error, $"Cannot start: '{ex.Message}'.");

                Close();
            }
        }

        private static void bindProcessEvents(Process process)
        {
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) =>
            {
                Close();
            };
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
