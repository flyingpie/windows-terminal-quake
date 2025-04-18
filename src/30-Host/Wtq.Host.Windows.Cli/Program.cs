using System.Threading.Tasks;
using Wtq.Services.Win32;

namespace Wtq.Host.Windows.Cli;

public static class Program
{
	//[STAThread]
	//public static void Main(string[] args) => new WtqWin32().Run(args);

	public static async Task Main(string[] args) => new Exp().RunAsync();
}