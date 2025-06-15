namespace Wtq.Services.CLI.Commands.Apps;

[Command<AppsCommand>]
public class CloseCommand(HttpClient client) : IAsyncCommand
{
	public async Task ExecuteAsync()
	{
		var r = await client.PostAsync("/apps/close", new StringContent(""));
	}
}