using Wtq.Host.Windows.Cli;

namespace Wtq.Host.Windows;

public static class Program
{
	[STAThread]
	public static void Main(string[] args) => new WtqWin32().Run(args);
}