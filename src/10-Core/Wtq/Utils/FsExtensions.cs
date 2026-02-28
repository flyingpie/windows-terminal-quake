namespace Wtq.Utils;

public static class FsExtensions
{
	public static string AssertFileExists(this string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		if (!Fs.Inst.FileExists(path))
		{
			throw new FsException($"File at path '{path}' does not exist.");
		}

		return path;
	}

	/// <summary>
	/// Make sure the specified <param name="path"/> exists.
	/// </summary>
	public static string EnsureDirExists(this string path)
	{
		if (Fs.Inst.DirExists(path))
		{
			return path;
		}

		try
		{
			Fs.Inst.CreateDir(path);
		}
		catch (Exception ex)
		{
			throw new FsException($"Could not create app data directory '{path}': {ex.Message}", ex);
		}

		return path;
	}

	/// <summary>
	/// Make sure the specified <param name="path"/> exists.
	/// </summary>
	public static string EnsureFileDirExists(this string path)
	{
		Guard.Against.NullOrWhiteSpace(path);

		var dir = Path.GetDirectoryName(path);

		if (string.IsNullOrWhiteSpace(dir))
		{
			throw new FsException($"Getting directory from path '{path}' resulted in an empty string.");
		}

		if (Fs.Inst.DirExists(dir))
		{
			return path;
		}

		try
		{
			Fs.Inst.CreateDir(dir);
		}
		catch (Exception ex)
		{
			throw new FsException($"Could not create directory '{dir}' for file at path '{path}': {ex.Message}", ex);
		}

		return path;
	}
}