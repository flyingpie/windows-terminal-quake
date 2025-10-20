namespace Wtq.Services.CLI.Commands.Apps;

[Command(Parent = typeof(AppsCommand))]
public class ListCommand(HttpClient client) : IAsyncCommand
{
	public async Task ExecuteAsync(CancellationToken ct = default)
	{
		var response = await client.GetStringAsync(new Uri("/apps", UriKind.Relative), ct).NoCtx();

		Console.WriteLine($"LIST APP {response}");
	}
}