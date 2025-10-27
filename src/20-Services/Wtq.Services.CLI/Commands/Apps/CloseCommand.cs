namespace Wtq.Services.CLI.Commands.Apps;

[Command(Parent = typeof(AppsCommand))]
public class CloseCommand(HttpClient client) : IAsyncCommand
{
	public async Task ExecuteAsync(CancellationToken ct = default)
	{
		using var content = new StringContent(string.Empty);

		_ = await client.PostAsync("/apps/close", content, ct).NoCtx();
	}
}