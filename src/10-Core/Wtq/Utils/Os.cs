using System.Runtime.InteropServices;

namespace Wtq.Utils;

public static class Os
{
	private const string WtqPlatformOverride = "WTQ_PLATFORM_OVERRIDE";

	/// <summary>
	/// When looking for the existence of a file and whether it's executable, we consider these extensions.
	/// </summary>
	private static readonly string[] ExeExts = [string.Empty, ".exe", ".bat", ".cmd"];

	public static bool IsFlatpak =>
		_isFlatpak ??= EnvUtils.GetEnvVar("container")?.Equals("flatpak", StringComparison.OrdinalIgnoreCase) ?? false;

	public static bool IsLinux =>
		EnvUtils.HasEnvVarWithValue(WtqPlatformOverride, "linux") // For testing purposes.
		|| RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

	public static bool IsWindows =>
		RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	public static bool IsCallable(string? workingDirectory, string fileName)
	{
		// Check whether the file exists in the working directory first.
		if (!string.IsNullOrWhiteSpace(workingDirectory) && IsExecutablePresent(workingDirectory, fileName))
		{
			return true;
		}

		return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
			? IsCallableLnx(fileName)
			: IsCallableWin(fileName);
	}

	public static bool IsCallableLnx(string fileName)
	{
		return File.Exists(fileName) || ExistsOnPath(fileName);
	}

	public static bool IsCallableWin(string fileName)
	{
		foreach (var ext in ExeExts)
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

	public static bool IsSymlink(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		if (!File.Exists(path))
		{
			throw new InvalidOperationException($"No such file at path '{path}'.");
		}

		var pathInfo = new FileInfo(path);

		return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
	}

	public static void OpenFileOrDirectory(string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		var log = Utils.Log.For(nameof(OpenFileOrDirectory));

		try
		{
			Process.Start(
				new ProcessStartInfo()
				{
					FileName = path, UseShellExecute = true,
				});
		}
		catch (Exception ex)
		{
			log.LogWarning(ex, "Could not open file or directory {Path}: {Message}", path, ex.Message);
		}
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

	private static bool IsExecutablePresent(string workingDirectory, string fileName)
	{
		foreach (var ext in ExeExts)
		{
			if (File.Exists(Path.Combine(workingDirectory, fileName + ext)))
			{
				return true;
			}
		}

		return false;
	}
}