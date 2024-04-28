using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Octokit;
using Octokit.Internal;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
	"ci",
	GitHubActionsImage.WindowsLatest,
	FetchDepth = 0,
	OnPushBranches = ["master"],
	InvokedTargets = [nameof(PublishAll)])]
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "MvdO: Build script.")]
public sealed class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.PublishAll);

	[Nuke.Common.Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Required]
	[GitRepository]
	private readonly GitRepository GitRepository;

	[MinVer]
	private readonly MinVer MinVer;

	[Nuke.Common.Parameter("GitHub Token")]
	private readonly string GitHubToken;

	private AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

	private AbsolutePath OutputDirectory => RootDirectory / "_output";

	private AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";

	private AbsolutePath StagingDirectory => OutputDirectory / "staging";

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	private readonly Solution Solution;

	private AbsolutePath PathToWin64FrameworkDependentZip => ArtifactsDirectory / $"win-x64_framework-dependent.zip";

	private AbsolutePath PathToWin64SelfContainedZip => ArtifactsDirectory / $"win-x64_self-contained.zip";

	private GitHubActions GitHubActions => GitHubActions.Instance;

	/// <summary>
	/// Clean output directories.
	/// </summary>
	private Target Clean => _ => _
		.Executes(() =>
		{
			OutputDirectory.CreateOrCleanDirectory();
		});

	/// <summary>
	/// NuGet restore.
	/// </summary>
	public Target Restore => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetRestore();
		});

	/// <summary>
	/// Windows x64 framework dependent.
	/// </summary>
	private Target PublishWin64FrameworkDependent => _ => _
		.DependsOn(Restore)
		.Produces(PathToWin64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(false));

			st.ZipTo(
				PathToWin64FrameworkDependentZip,
				filter: x => x.HasExtension(".exe", ".jsonc"),
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: System.IO.FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 self contained.
	/// </summary>
	private Target PublishWin64SelfContained => _ => _
		.DependsOn(Restore)
		.Produces(PathToWin64SelfContainedZip)
		.Executes(() =>
		{
			var staging = StagingDirectory / "win-x64_self-contained";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(staging)
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(true));

			staging.ZipTo(
				PathToWin64SelfContainedZip,
				filter: x => x.HasExtension(".exe", ".jsonc"),
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: System.IO.FileMode.CreateNew);
		});

	/// <summary>
	/// Scoop manifest.
	/// </summary>
	private Target CreateScoopManifest => _ => _
		.Executes(async () =>
		{
			var tpl = await File.ReadAllTextAsync(RootDirectory / "scoop" / "_template.json");
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			var manifest = tpl
				.Replace("$PACKAGE_VERSION$", MinVer.MinVerVersion, StringComparison.OrdinalIgnoreCase)
				.Replace("$GH_RELEASE_VERSION$", $"v{MinVer.MinVerVersion}", StringComparison.OrdinalIgnoreCase)
				.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-latest.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-nightly.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / $"wtq-{MinVer.MinVerVersion}.json", manifest);
		});

	/// <summary>
	/// WinGet manifest.
	/// </summary>
	private Target CreateWinGetManifest => _ => _
		.Executes(async () =>
		{
			var templateRoot = RootDirectory / "winget" / "_template";
			var manifestRoot = RootDirectory / "winget" / MinVer.MinVerVersion;
			var prefix = "flyingpie.windows-terminal-quake";
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			if (Directory.Exists(manifestRoot)) { Directory.Delete(manifestRoot, true); }

			Directory.CreateDirectory(manifestRoot);

			var installerFn = $"{prefix}.installer.yaml";
			var localeFn = $"{prefix}.locale.en-US.yaml";
			var mainFn = $"{prefix}.yaml";

			var fns = new string[] { installerFn, localeFn, mainFn };

			foreach (var fn in fns)
			{
				var tpl = await File.ReadAllTextAsync(templateRoot / fn);
				var target = manifestRoot / fn;

				var manifest = tpl
					.Replace("$PACKAGE_VERSION$", MinVer.MinVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$GH_RELEASE_VERSION$", $"v{MinVer.MinVerVersion}", StringComparison.OrdinalIgnoreCase)
					.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

				await File.WriteAllTextAsync(target, manifest);
			}
		});

	/// <summary>
	/// GitHub release.
	/// </summary>
	private Target CreateGitHubRelease => _ => _
		.Description($"Creating release for the publishable version.")
		.OnlyWhenDynamic(() => Configuration.Equals(Configuration.Release))
		.Executes(async () =>
		{
			var token = GitHubActions?.Token ?? GitHubToken
				?? throw new InvalidOperationException("No GitHub token was specified.");

			var credentials = new Credentials(token);

			GitHubTasks.GitHubClient =
				new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

			var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

			var ghRelease = await GitHubTasks.GitHubClient.GetOrCreateGitHubReleaseAsync(owner, name, MinVer.MinVerVersion);

			// Update release notes.
			var latestChangeLog = await NukeExtensions.GetChangeLogEntryAsync(ChangeLogFile, MinVer);

			await GitHubTasks
				.GitHubClient
				.Repository
				.Release
				.Edit(owner, name, ghRelease.Id, new ReleaseUpdate()
				{
					Body = latestChangeLog,
					TargetCommitish = GitRepository.Commit,
					Draft = false,
					Prerelease = true,
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
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64FrameworkDependentZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64SelfContainedZip);
		});

	private Target PublishAll => _ => _
		.DependsOn(Clean)
		.DependsOn(PublishWin64FrameworkDependent)
		.DependsOn(PublishWin64SelfContained)
		.Triggers(CreateScoopManifest)
		.Triggers(CreateWinGetManifest)
		.Triggers(CreateGitHubRelease)
		.Executes();
}