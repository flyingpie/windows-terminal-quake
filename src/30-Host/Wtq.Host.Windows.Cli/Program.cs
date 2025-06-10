namespace Wtq.Host.Windows.Cli;

public static class Program
{
	[STAThread]
	public static void Main(string[] args) => new WtqWin32().RunAsync(args).GetAwaiter().GetResult();
}