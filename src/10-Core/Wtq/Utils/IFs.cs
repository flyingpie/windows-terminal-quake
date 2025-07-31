namespace Wtq.Utils;

public interface IFs
{
	string CreateDir(string path);

	bool DirExists(string path);

	bool FileExists(string path);
}