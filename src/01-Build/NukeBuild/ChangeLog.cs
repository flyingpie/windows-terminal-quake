#pragma warning disable
// ReSharper disable All

using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public static partial class ChangeLog
{
	public static async Task<string> GetEntryAsync(
		string path,
		string version)
	{
		var inCurr = false;
		var headerRegex = ChangeLogVersionHeaderRegex();

		var res = new StringBuilder();

		await foreach (var line in File.ReadLinesAsync(path))
		{
			// See if this line is a version header.
			var match = headerRegex.Match(line);
			if (match.Success)
			{
				// See if this version header matches the one we're publishing.
				// If it is, we need to start reading the contents.
				if (match.Groups["semver"].Value == version)
				{
					inCurr = true;
					continue;
				}

				// If it doesn't, and we were reading the contents, we're done reading the changelog.
				if (inCurr)
				{
					break;
				}
			}

			if (inCurr)
			{
				res.AppendLine(line);
			}
		}

		return res.ToString();
	}


	[GeneratedRegex(@"^## \[(?<semver>.+?)\]")]
	private static partial Regex ChangeLogVersionHeaderRegex();
}