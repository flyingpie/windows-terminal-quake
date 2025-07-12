namespace Wtq.Host.Linux;

public static class Program
{
	public static Task Main(string[] args) => new WtqLinux().RunAsync(args);
}