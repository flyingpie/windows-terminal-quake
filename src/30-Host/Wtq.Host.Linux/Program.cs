namespace Wtq.Host.Linux;

public static class Program
{
	public static void Main(string[] args) => new WtqLinux().RunAsync(args).GetAwaiter().GetResult();
}