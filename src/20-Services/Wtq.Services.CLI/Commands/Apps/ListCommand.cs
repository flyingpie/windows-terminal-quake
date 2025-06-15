namespace Wtq.Services.CLI.Commands.Apps;

[Command<AppsCommand>]
public class ListCommand(HttpClient client) : IAsyncCommand
{
	public async Task ExecuteAsync()
	{
		var r = await client.GetStringAsync("/apps");

		Console.WriteLine($"LIST APP {r}");
	}
}