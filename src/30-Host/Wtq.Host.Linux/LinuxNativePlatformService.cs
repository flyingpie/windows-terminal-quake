// using Wtq.Services;
//
// namespace Wtq.Host.Linux;
//
// public class LinuxNativePlatformService : IPlatformService
// {
// 	public const string Config = "WTQ_CONFIG_FILE";
//
// 	public string PlatformName { get; } = "Linux Native";
//
// 	public string PathToLogs { get; }
//
// 	public string PathToWtqConf { get; set; }
//
// 	public ICollection<string> PathsToWtqConfs { get; } =
// 	[
//
// 	];
//
// 	public string PreferredPathWtqConfig { get; }
//
// 	public bool IsCallable(string? workingDirectory, string fileName)
// 	{
// 		throw new NotImplementedException();
// 	}
//
// 	public bool ShouldUsePollingFileWatcherForPath(string path)
// 	{
// 		throw new NotImplementedException();
// 	}
//
// 	public void OpenFileOrDirectory(string path)
// 	{
// 		throw new NotImplementedException();
// 	}
//
// 	public void OpenUrl(Uri url)
// 	{
// 		throw new NotImplementedException();
// 	}
// }