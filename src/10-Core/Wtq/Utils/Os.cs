using System.Runtime.InteropServices;

namespace Wtq.Utils;

public static class Os
{
	public static bool IsCallable(string fileName)
	{
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return IsCallableLnx(fileName);
		}

		return IsCallableWin(fileName);
	}

	public static bool IsCallableLnx(string fileName)
	{
		if (File.Exists(fileName))
		{
			return true;
		}

		return ExistsOnPath(fileName);
	}

	public static bool IsCallableWin(string fileName)
	{
		string[] exeExts = [string.Empty, ".exe", ".bat", ".cmd"];

		foreach (var ext in exeExts)
		{
			var path = fileName + ext;

			if (File.Exists(path))
			{
				return true;
			}

			if (ExistsOnPath(path))
			{
				return true;
			}
		}

		return false;
	}

	public static bool ExistsOnPath(string fileName)
	{
		return GetFullPath(fileName) != null;
	}

	public static string? GetFullPath(string fileName)
	{
		if (File.Exists(fileName))
		{
			return Path.GetFullPath(fileName);
		}

		var values = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
		foreach (var path in values.Split(Path.PathSeparator))
		{
			var fullPath = Path.Combine(path, fileName);
			if (File.Exists(fullPath))
			{
				return fullPath;
			}
		}

		return null;
	}

	public static void OpenFileOrDirectory(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		Process.Start(
			new ProcessStartInfo()
			{
				FileName = path, UseShellExecute = true,
			});
	}

	public static void OpenUrl(Uri url)
	{
		Guard.Against.Null(url);

		try
		{
			Process.Start(url.ToString());
		}
		catch
		{
			// Hack because of this: https://github.com/dotnet/corefx/issues/10361
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var urlstr = url.ToString().Replace("&", "^&", StringComparison.Ordinal);
				Process.Start(
					new ProcessStartInfo(urlstr)
					{
						UseShellExecute = true,
					});
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				Process.Start("xdg-open", url.ToString());
			}
			else
			{
				throw;
			}
		}
	}
}