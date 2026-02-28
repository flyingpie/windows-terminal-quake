#pragma warning disable
// ReSharper disable All

using Nuke.Common;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Octokit.Internal;
using Serilog;
using System;

public sealed partial class Build
{
	/// <summary>
	/// GitHub release.
	/// </summary>
	private Target CreateGitHubRelease => _ => _
		.Description($"Creating release for the publishable version.")
		.Executes(async () =>
		{
			if (GitRepository == null)
			{
				throw new InvalidOperationException($"The '{nameof(GitRepository)}' property was not set, which is required when making a GitHub release.");
			}

			var token = GitHubActions?.Token ?? GitHubToken
				?? throw new InvalidOperationException("No GitHub token was specified.");

			var credentials = new Credentials(token);

			GitHubTasks.GitHubClient =
				new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

			var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

			var ghRelease = await GitHubTasks.GitHubClient.GetOrCreateGitHubReleaseAsync(owner, name, GitHubRelease);

			// Update release notes.
			var latestChangeLog = await ChangeLog.GetEntryAsync(ChangeLogFile, GitHubRelease);

			await GitHubTasks
				.GitHubClient
				.Repository
				.Release
				.Edit(owner, name, ghRelease.Id, new ReleaseUpdate()
				{
					Body = latestChangeLog, TargetCommitish = GitRepository.Commit, Draft = false, Prerelease = true,
				});

			// Delete existing assets.
			foreach (var ass in ghRelease.Assets)
			{
				Log.Information("Deleting existing asset '{AssetName}'", ass.Name);

				await GitHubTasks
					.GitHubClient
					.Repository
					.Release
					.DeleteAsset(owner, name, ass.Id);
			}

			// Upload new assets.
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64FrameworkDependentZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64FrameworkDependentZipSha256);

			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64SelfContainedZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64SelfContainedZipSha256);

			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64FrameworkDependentZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64FrameworkDependentZipSha256);

			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64SelfContainedZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64SelfContainedZipSha256);
		});
}