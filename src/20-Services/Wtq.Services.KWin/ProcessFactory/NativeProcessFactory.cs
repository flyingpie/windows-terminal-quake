// using Wtq.Configuration;
//
// namespace Wtq.Services.KWin.ProcessFactory;
//
// /// <summary>
// /// Creates a process start info object for when WTQ has direct access to the host OS, runs as a native (non-sandboxed) app.
// /// </summary>
// public class NativeProcessFactory : IProcessFactory
// {
// 	private readonly ILogger _log = Log.For<NativeProcessFactory>();
//
// 	public Process Create(WtqAppOptions opts)
// 	{
// 		Guard.Against.Null(opts);
//
// 		if (string.IsNullOrWhiteSpace(opts.FileName))
// 		{
// 			throw new InvalidOperationException($"Cannot start process for app '{opts.Name}': missing required property '{nameof(opts.FileName)}'");
// 		}
//
// 		var startInfo = new ProcessStartInfo()
// 		{
// 			FileName = opts.FileName,
// 			Arguments = opts.Arguments,
// 			WorkingDirectory = opts.WorkingDirectory,
// 		};
//
// 		// Arguments
// 		foreach (var arg in opts.ArgumentList)
// 		{
// 			if (string.IsNullOrWhiteSpace(arg.Argument))
// 			{
// 				continue;
// 			}
//
// 			var exp = arg.Argument.ExpandEnvVars();
//
// 			_log.LogDebug("Adding process argument '{ArgumentOriginal}', expanded to '{ArgumentExpanded}'", arg, exp);
//
// 			startInfo.ArgumentList.Add(exp);
// 		}
//
// 		return new Process()
// 		{
// 			StartInfo = startInfo,
// 		};
// 	}
// }