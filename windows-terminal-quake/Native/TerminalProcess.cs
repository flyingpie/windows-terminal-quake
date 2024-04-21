using WindowsTerminalQuake.TerminalProcessProviders;

namespace WindowsTerminalQuake.Native;

/// <summary>
/// Wrapper around the Windows Terminal process. Contains stuff to actually capture the process,
/// which turns out to be tricky in some cases.
/// </summary>
public static class TerminalProcess
{
	//private static IProcessProvider? _provider;

	private static List<(App app, IProcessProvider pp)> _apps = new();

	public static int _currentApp;

	public static void Init()
	{
		// TODO: What if no apps?
		// TODO: What if process stops?
		// TODO: What if settings change, apps add/removed/modified/etc.?

		foreach (var a in QSettings.Instance.Apps)
		{
			var pp = a.CreateProcessProvider();

			_apps.Add((a, pp));

			pp.Get();
		}
	}

	/// <summary>
	/// Returns the instance of the running Windows Terminal. Creates one if none is running yet.
	/// </summary>
	/// <param name="args">Any command-line arguments to pass to the terminal process if we're starting it.</param>
	public static Process Get(string[] args)
	{
		//return GetProcessProvider().Get(args);

		return _apps[_currentApp].pp.Get();
	}

	public static void OnExit(Action action)
	{
		//GetProcessProvider().OnExit(action);
	}

	//private static IProcessProvider GetProcessProvider()
	//{
	//	if (_provider == null)
	//	{
	//		//var providerName = QSettings.Instance.ProcessProvider?.Trim() ?? string.Empty;

	//		//if (providerName.Equals(nameof(GenericProcessProvider), StringComparison.OrdinalIgnoreCase))
	//		{
	//			_provider = new GenericProcessProvider();
	//		}
	//		//else
	//		//{
	//		//	_provider = new WindowsTerminalProcessProvider();
	//		//}
	//	}

	//	return _provider;
	//}
}