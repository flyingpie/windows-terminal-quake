using Gn = Wtq.WtqConstants.Settings.GroupNames;

namespace Wtq.Configuration;

/// <summary>
/// A single action to undertake when a specified event is fired.
/// </summary>
public class WtqAppEventHookOptions
{
	private readonly ILogger _log = Log.For<WtqAppEventHookOptions>();
	private ICollection<string> _argumentList = [];

	/// <summary>
	/// A pattern that determines what event or events to hit.<br/>
	/// Supports regular expressions.
	/// </summary>
	[Display(GroupName = Gn.Events, Name = "Event pattern")]
	[JsonPropertyOrder(7001)]
	[Required]
	public string EventPattern { get; set; } = null!;

	/// <summary>
	/// The filename used when starting a process, when the event occurs.
	/// </summary>
	[Display(GroupName = Gn.Events, Name = "Filename")]
	[JsonPropertyOrder(7002)]
	[Required]
	public string FileName { get; set; } = null!;

	/// <summary>
	/// Command-line arguments that should be passed to the process when it's started.
	/// </summary>
	[Display(GroupName = Gn.Events, Name = "Argument list")]
	[JsonPropertyOrder(7003)]
	public ICollection<string> ArgumentList
	{
		get => _argumentList;
		set => _argumentList = value ?? [];
	}

	public async Task ExecuteAsync(IDictionary<string, object?> parameters)
	{
		_log.LogInformation("Running event hook '{EventHook}'", this);

		try
		{
			// Keep track of how long the action took, for troubleshooting purposes.
			var sw = Stopwatch.StartNew();

			// Create a process (doesn't actually start it yet).
			var p = new Process();

			// Add all parameters as environment variables, so they can be used in scripts and such.
			foreach (var k in parameters)
			{
				// Prefix each environment variable to prevent overlap with existing ones.
				p.StartInfo.Environment[$"WTQ_{k.Key.ToSnakeCase().ToUpperInvariant()}"] = k.Value?.ToString() ?? string.Empty;
			}

			// Add filename and arguments, both support environment variables.
			p.StartInfo.FileName = FileName.ExpandEnvVars();
			foreach (var a in _argumentList.Where(a => !string.IsNullOrWhiteSpace(a)))
			{
				p.StartInfo.ArgumentList.Add(a.ExpandEnvVars());
			}

			// Execute the process in the background, prevents command prompts from popping up on Windows.
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;

			// Pipe the output to our log.
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.RedirectStandardOutput = true;

			p.OutputDataReceived += (_, a) =>
			{
				if (!string.IsNullOrWhiteSpace(a.Data))
				{
					_log.LogDebug(a.Data);
				}
			};

			p.ErrorDataReceived += (_, a) =>
			{
				if (!string.IsNullOrWhiteSpace(a.Data))
				{
					_log.LogWarning(a.Data);
				}
			};

			// Now to actually start the process.
			var result = p.Start();

			p.BeginErrorReadLine();
			p.BeginOutputReadLine();

			// Shouldn't run for too long.
			await p.WaitForExitAsync();

			_log.LogInformation("Finished event hook '{EventHook}', result: {Result}, took {Elapsed}", this, result, sw.Elapsed);
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Error executing event hook '{EventHook}': {Message}", this, ex.Message);
		}
	}

	public override string ToString() => FileName;
}