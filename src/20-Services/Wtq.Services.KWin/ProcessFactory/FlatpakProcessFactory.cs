using Wtq.Configuration;

namespace Wtq.Services.KWin.ProcessFactory;

/// <summary>
/// Creates a process start info object for when WTQ runs as a Flatpak.
/// </summary>
public class FlatpakProcessFactory : IProcessFactory
{
	private readonly ILogger _log = Log.For<FlatpakProcessFactory>();

	public Process Create(WtqAppOptions opts)
	{
		Guard.Against.Null(opts);

		if (string.IsNullOrWhiteSpace(opts.FileName))
		{
			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'");
		}

		var startInfo = new ProcessStartInfo()
		{
			FileName = "flatpak-spawn",
		};

		startInfo.ArgumentList.Add("--host");

		// Working directory
		if (!string.IsNullOrWhiteSpace(opts.WorkingDirectory))
		{
			startInfo.ArgumentList.Add("--directory");
			startInfo.ArgumentList.Add(opts.WorkingDirectory);
		}

		// Filename
		startInfo.ArgumentList.Add(opts.FileName);

		// Arguments
		foreach (var arg in opts.ArgumentList ?? [])
		{
			if (string.IsNullOrWhiteSpace(arg.Argument))
			{
				continue;
			}

			var exp = arg.Argument.ExpandEnvVars();

			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg.Argument, exp);

			startInfo.ArgumentList.Add(exp);
		}

		return new Process()
		{
			StartInfo = startInfo,
		};
	}
}