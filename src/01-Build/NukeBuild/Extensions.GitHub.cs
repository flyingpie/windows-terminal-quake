#pragma warning disable
// ReSharper disable All

using Octokit;
using Serilog;
using System.IO;
using System.Threading.Tasks;

public static class ExtensionsGitHub
{
	private static readonly string ApplicationOctetStream = "application/octet-stream";

	public static async Task<Release> GetOrCreateGitHubReleaseAsync(
		this GitHubClient client,
		string owner,
		string name,
		string version)
	{
		try
		{
			Log.Information("Looking for GitHub release '{TagName}'", version);
			var latestRelease = await client.Repository.Release.Get(owner, name, version);

			// Latest release should never be null, but just for clarity sake.
			if (latestRelease != null)
			{
				Log.Information("Found existing GitHub release '{TagName}'", version);
				return latestRelease;
			}
		}
		catch (NotFoundException)
		{
			// No release exists yet, create one now.
		}

		// Create release if none exists yet.
		Log.Information("Creating GitHub release '{TagName}'", version);

		var newRelease = new NewRelease(version)
		{
			Name = version, Prerelease = true,
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
			FileName = fileName, ContentType = ApplicationOctetStream, RawData = artifactStream,
		};

		await client.Repository.Release.UploadAsset(release, assetUpload);
	}
}