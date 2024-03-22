using System.IO;
using Wtq.Core.Exceptions;

namespace Wtq.Core;

public static class App
{
	private static string? _pathToAppConf;
	private static string? _pathToAppDir;
	private static string? _pathToAppExe;

	public static string PathToAppConf
	{
		get
		{
			if (_pathToAppConf == null)
			{
				_pathToAppConf = Path.Combine(PathToAppDir, "wtq.jsonc");
			}

			return _pathToAppConf;
		}
	}

	public static string PathToAppDir
	{
		get
		{
			if (_pathToAppDir == null)
			{
				_pathToAppDir = Path.GetDirectoryName(PathToAppExe);
			}

			return _pathToAppDir;
		}
	}

	public static string PathToAppExe
	{
		get
		{
			if (_pathToAppExe == null)
			{
				_pathToAppExe = Environment.ProcessPath
					?? throw new WtqException("Could not find path to wtq exe.");
			}

			return _pathToAppExe;
		}
	}
}