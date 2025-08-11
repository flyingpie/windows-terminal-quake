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

	public string ReadAllText(string path) => File.ReadAllText(path);

	public void WriteAllText(string path, string contents) => File.WriteAllText(path, contents);
}