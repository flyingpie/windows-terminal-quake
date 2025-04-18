namespace Wtq.Configuration;

public class WtqAppEventHookOptions
{
	private readonly ILogger _log = Log.For<WtqAppEventHookOptions>();
	private ICollection<string> _argumentList = [];

	public string FileName { get; set; }

	public ICollection<string> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	public Task ExecuteAsync()
	{
		try
		{
			Process.Start(FileName, _argumentList);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error executing event hook '{EventHook}': {Message}", this, ex.Message);
		}

		return Task.CompletedTask;
	}

	public override string ToString() => $"{FileName} {string.Join(" ", ArgumentList)}";
}