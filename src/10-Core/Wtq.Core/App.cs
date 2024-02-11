using System.IO;
using Wtq.Core.Exceptions;

namespace Wtq.Core;

public static class App
{
	private static string? _pathToAppDir;

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

	private static string? _pathToAppExe;

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