using Octokit;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "MvdO: Build script.")]
public static partial class NukeExtensions
{
	private static readonly string ApplicationOctetStream = "application/octet-stream";

	public static async Task<string> GetChangeLogEntryAsync(
		string path,
		string ver)
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
				if (match.Groups["semver"].Value == ver)
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

	public static async Task<Release> GetOrCreateGitHubReleaseAsync(
		this GitHubClient client,
		string owner,
		string name,
		string version)
	{
		var tagName = $"v{version}";

		try
		{
			Log.Information("Looking for GitHub release '{TagName}'", tagName);
			var latestRelease = await client.Repository.Release.Get(owner, name, tagName);

			// Latest release should never be null, but just for clarity sake.
			if (latestRelease != null)
			{
				Log.Information("Found existing GitHub release '{TagName}'", tagName);
				return latestRelease;
			}
		}
		catch (NotFoundException)
		{
			// No release exists yet, create one now.
		}

		// Create release if none exists yet.
		Log.Information("Creating GitHub release '{TagName}'", tagName);

		var newRelease = new NewRelease(tagName)
		{
			Name = tagName,
			Prerelease = true,
		};

		return await client
			.Repository
			.Release.Create(owner, name, newRelease);
	}

	public static async Task UploadReleaseAssetToGithub(
		this GitHubClient client,
		Release release,
		string asset)
	{
		Log.Information("Uploading asset '{AssetName}'", asset);

		await using var artifactStream = File.OpenRead(asset);
		var fileName = Path.GetFileName(asset);
		var assetUpload = new ReleaseAssetUpload
		{
			FileName = fileName,
			ContentType = ApplicationOctetStream,
			RawData = artifactStream,
		};

		await client.Repository.Release.UploadAsset(release, assetUpload);
	}

	[GeneratedRegex(@"^## \[(?<semver>\d+\.?\d+\.?\d+)\]")]
	private static partial Regex ChangeLogVersionHeaderRegex();
}