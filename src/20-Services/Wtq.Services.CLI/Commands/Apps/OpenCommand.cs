namespace Wtq.Services.CLI.Commands.Apps;

[Command<AppsCommand>]
public class OpenCommand(HttpClient client) : IAsyncCommand
{
	[Option(IsRequired = true)]
	public string App { get; set; } = null!;

	public async Task ExecuteAsync()
	{
		var r = await client.PostAsync($"/apps/open?appName={App}", new StringContent(""));
	}
}