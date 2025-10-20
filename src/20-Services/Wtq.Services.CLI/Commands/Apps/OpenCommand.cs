namespace Wtq.Services.CLI.Commands.Apps;

[Command(Parent = typeof(AppsCommand))]
public class OpenCommand(HttpClient client) : IAsyncCommand
{
	[Option(Required = true)]
	public string App { get; set; } = null!;

	public async Task ExecuteAsync(CancellationToken ct = default)
	{
		using var content = new StringContent(string.Empty);

		_ = await client.PostAsync(new Uri($"/apps/open?appName={App}", UriKind.Relative), content, ct).NoCtx();
	}
}