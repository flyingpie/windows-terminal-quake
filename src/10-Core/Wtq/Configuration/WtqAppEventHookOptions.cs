using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

public class WtqAppEventHookOptions
{
	private readonly ILogger _log = Log.For<WtqAppEventHookOptions>();
	private ICollection<string> _argumentList = [];

	/// <summary>
	/// Which monitor to preferably drop the app.
	/// </summary>
	// [DefaultValue(Wc.PreferMonitor.WithCursor)]
	[Display(GroupName = Gn.Monitor, Name = "Filename")]
	[JsonPropertyOrder(7001)]
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
		_log.LogInformation("Running event hook '{EventHook}'", this);

		try
		{
			var sw = Stopwatch.StartNew();

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
			p.StartInfo.UseShellExecute = false;

			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;

			p.OutputDataReceived += (s, a) => _log.LogInformation(a.Data);
			p.ErrorDataReceived += (s, a) => _log.LogWarning(a.Data);

			var result = p.Start();

			p.BeginErrorReadLine();
			p.BeginOutputReadLine();

			await p.WaitForExitAsync();

			_log.LogInformation("Finished event hook '{EventHook}', result: {Result}, took {Elapsed}", this, result, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error executing event hook '{EventHook}': {Message}", this, ex.Message);
		}
	}

	public override string ToString() => $"{FileName} {string.Join(" ", ArgumentList)}";
}