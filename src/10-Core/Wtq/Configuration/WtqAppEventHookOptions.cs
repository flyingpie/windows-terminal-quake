namespace Wtq.Configuration;

public class WtqAppEventHookOptions
{
	private readonly ILogger _log = Log.For<WtqAppEventHookOptions>();
	private ICollection<string> _argumentList = [];

	[Required]
	public string FileName { get; set; } = null!;

	public ICollection<string> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	public bool StopPropagation { get; set; }

	public async Task ExecuteAsync(IDictionary<string, object?> parameters)
	{
		try
		{
			var p = new Process();
			foreach (var k in parameters)
			{
				p.StartInfo.Environment[$"WTQ_{k.Key.ToSnakeCase().ToUpperInvariant()}"] = k.Value?.ToString() ?? string.Empty;
			}

			p.StartInfo.FileName = FileName;
			foreach (var a in _argumentList.Where(a => !string.IsNullOrWhiteSpace(a)))
			{
				p.StartInfo.ArgumentList.Add(a.ExpandEnvVars());
			}

			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = true;

			var result = p.Start();

			_log.LogInformation("Starting process (RESULT:{Result})", result);

			await p.WaitForExitAsync();
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error executing event hook '{EventHook}': {Message}", this, ex.Message);
		}
	}

	public override string ToString() => $"{FileName} {string.Join(" ", ArgumentList)}";
}