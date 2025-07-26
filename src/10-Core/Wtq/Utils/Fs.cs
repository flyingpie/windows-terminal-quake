namespace Wtq.Utils;

public class Fs : IFs
{
	public static IFs Inst { get; set; } = new Fs();

	public string CreateDir(string path)
	{
		Directory.CreateDirectory(path);

		return path;
	}

	public bool DirExists(string path) => Directory.Exists(path);

	public bool FileExists(string path) => File.Exists(path);
}

public interface IFs
{
	string CreateDir(string path);

	bool DirExists(string path);

	bool FileExists(string path);
}

public sealed class FsException : Exception
{
	public FsException(string message)
		: base(message)
	{
	}

	public FsException(string message, Exception ex)
		: base(message, ex)
	{
	}
}

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
}