using System;
using System.Threading.Tasks;

namespace Wtq.Host.Linux;

public static class Program
{
	public static async Task Main(string[] args)
	{
		Console.WriteLine("PRE MAIN()");

		await new WtqLinux().RunAsync(args);

		Console.WriteLine("POST MAIN()");
	}
}