using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
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
	OnWorkflowDispatchOptionalInputs = [ "name" ],
	EnableGitHubToken = true,
	InvokedTargets = [nameof(PublishRelease)])]	
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "MvdO: Build script.")]
public sealed class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.PublishDebug);

	[Nuke.Common.Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Nuke.Common.Parameter("GitHubRelease")]
	private string GitHubRelease = "vNext";

	[Nuke.Common.Parameter("Version")]
	private string SemVerVersion => "9.9.9";

	[Nuke.Common.Parameter("GitHub Token")]
	private readonly string GitHubToken;

	[Required]
	[GitRepository]
	private readonly GitRepository GitRepository;

	private AbsolutePath ChangeLogFile => RootDirectory / "CHANGELOG.md";

	private AbsolutePath OutputDirectory => RootDirectory / "_output";

	private AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";

	private AbsolutePath StagingDirectory => OutputDirectory / "staging";

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	private readonly Solution Solution;

	private AbsolutePath PathToLinux64FrameworkDependentZip => ArtifactsDirectory / "linux-x64_framework-dependent.tar.gz";

	private AbsolutePath PathToLinux64SelfContainedZip => ArtifactsDirectory / "linux-x64_self-contained.tar.gz";

	private AbsolutePath PathToWin64FrameworkDependentZip => ArtifactsDirectory / "win-x64_framework-dependent.zip";

	private AbsolutePath PathToWin64SelfContainedZip => ArtifactsDirectory / "win-x64_self-contained.zip";

	private GitHubActions GitHubActions => GitHubActions.Instance;

	/// <summary>
	/// Returns the version for use in assembly versioning.
	/// </summary>
	private string InformationalVersion => $"{SemVerVersion}.{DateTimeOffset.UtcNow:yyyyMMdd}+{GitRepository.Commit}";

	private Target ReportInfo => _ => _
		.Executes(() =>
		{
			Log.Information("Configuration:.........{Configuration}", Configuration);

			Log.Information("GitHubRelease:.........{GitHubRelease}", GitHubRelease);
			Log.Information("SemVerVersion:.........{SemVerVersion}", SemVerVersion);
			Log.Information("InformationalVersion:..{InformationalVersion}", InformationalVersion);

			Log.Information("OutputDirectory:.......{OutputDirectory}", OutputDirectory);
			Log.Information("ArtifactsDirectory:....{ArtifactsDirectory}", ArtifactsDirectory);
			Log.Information("StagingDirectory:......{StagingDirectory}", StagingDirectory);
		});

	/// <summary>
	/// Clean output directories.
	/// </summary>
	private Target Clean => _ => _
		.DependsOn(ReportInfo)
		.Executes(() =>
		{
			OutputDirectory.CreateOrCleanDirectory();
		});

	/// <summary>
	/// Tests.
	/// </summary>
	private Target RunTests => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetTest(_ => _
				.SetProjectFile(Solution.Path)
				.SetVerbosity(DotNetVerbosity.normal)
			);
		});

	/// <summary>
	/// Linux x64 framework dependent.
	/// </summary>
	private Target PublishLinux64FrameworkDependent => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToLinux64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "linux-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetAssemblyVersion(SemVerVersion)
				.SetInformationalVersion(InformationalVersion)
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st));

			st.DeleteUnnecessaryFiles();

			st.MoveWtqUI();

			st.TarGZipTo(
				PathToLinux64FrameworkDependentZip,
				fileMode: System.IO.FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 self-contained.
	/// </summary>
	private Target PublishLinux64SelfContained => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToLinux64SelfContainedZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "linux-x64_self-contained";

			DotNetPublish(_ => _
				.SetAssemblyVersion(SemVerVersion)
				.SetInformationalVersion(InformationalVersion)
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st)
				.SetRuntime("linux-x64")
				.SetSelfContained(true));

			st.DeleteUnnecessaryFiles();

			st.MoveWtqUI();

			st.TarGZipTo(
				PathToLinux64SelfContainedZip,
				fileMode: System.IO.FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 framework dependent.
	/// </summary>
	private Target PublishWin64FrameworkDependent => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToWin64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetAssemblyVersion(SemVerVersion)
				.SetInformationalVersion(InformationalVersion)
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetRuntime("win-x64")
				.SetSelfContained(false));

			st.DeleteUnnecessaryFiles();

			st.MoveWtqUI();

			st.ZipTo(
				PathToWin64FrameworkDependentZip,
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: System.IO.FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 self-contained.
	/// </summary>
	private Target PublishWin64SelfContained => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToWin64SelfContainedZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_self-contained";

			DotNetPublish(_ => _
				.SetAssemblyVersion(SemVerVersion)
				.SetInformationalVersion(InformationalVersion)
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetRuntime("win-x64")
				.SetSelfContained(true));

			st.DeleteUnnecessaryFiles();

			st.MoveWtqUI();

			st.ZipTo(
				PathToWin64SelfContainedZip,
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: System.IO.FileMode.CreateNew);
		});

	private Target PublishLinux64 => _ => _
		.DependsOn(Clean)
		.DependsOn(PublishLinux64FrameworkDependent)
		.DependsOn(PublishLinux64SelfContained)
		.Executes();

	private Target PublishWin64 => _ => _
		.DependsOn(Clean)
		.DependsOn(PublishWin64FrameworkDependent)
		.DependsOn(PublishWin64SelfContained)
		.Executes();

	/// <summary>
	/// Scoop manifest.
	/// </summary>
	private Target CreateScoopManifest => _ => _
		.DependsOn(PublishWin64SelfContained)
		.Executes(async () =>
		{
			var tpl = await File.ReadAllTextAsync(RootDirectory / "scoop" / "_template.json");
			var sha256 = Convert.ToHexString(await SHA256.HashDataAsync(File.OpenRead(PathToWin64SelfContainedZip))).ToLowerInvariant();

			var manifest = tpl
				.Replace("$GH_RELEASE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
				.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
				.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-latest.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / "wtq-nightly.json", manifest);
			await File.WriteAllTextAsync(RootDirectory / "scoop" / $"wtq-{SemVerVersion}.json", manifest);
		});

	/// <summary>
	/// WinGet manifest.
	/// </summary>
	private Target CreateWinGetManifest => _ => _
		.DependsOn(PublishWin64SelfContained)
		.Executes(async () =>
		{
			var templateRoot = RootDirectory / "winget" / "_template";
			var manifestRoot = RootDirectory / "winget" / SemVerVersion;
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
					.Replace("$GH_RELEASE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$PACKAGE_VERSION$", SemVerVersion, StringComparison.OrdinalIgnoreCase)
					.Replace("$RELEASE_DATE$", DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"), StringComparison.OrdinalIgnoreCase)
					.Replace("$SELF_CONTAINED_SHA256$", sha256, StringComparison.OrdinalIgnoreCase);

				await File.WriteAllTextAsync(target, manifest);
			}
		});

	/// <summary>
	/// GitHub release.
	/// </summary>
	private Target CreateGitHubRelease => _ => _
		.DependsOn(PublishLinux64)
		.DependsOn(PublishWin64)
		.Description($"Creating release for the publishable version.")
		.Executes(async () =>
		{
			var token = GitHubActions?.Token ?? GitHubToken
				?? throw new InvalidOperationException("No GitHub token was specified.");

			var credentials = new Credentials(token);

			GitHubTasks.GitHubClient =
				new GitHubClient(new ProductHeaderValue(nameof(NukeBuild)), new InMemoryCredentialStore(credentials));

			var (owner, name) = (GitRepository.GetGitHubOwner(), GitRepository.GetGitHubName());

			var ghRelease = await GitHubTasks.GitHubClient.GetOrCreateGitHubReleaseAsync(owner, name, GitHubRelease);

			// Update release notes.
			var latestChangeLog = await NukeExtensions.GetChangeLogEntryAsync(ChangeLogFile, GitHubRelease);

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
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64FrameworkDependentZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToLinux64SelfContainedZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64FrameworkDependentZip);
			await GitHubTasks.GitHubClient.UploadReleaseAssetToGithub(ghRelease, PathToWin64SelfContainedZip);
		});

	private Target PublishDebug => _ => _
		.DependsOn(PublishLinux64FrameworkDependent)
		.DependsOn(PublishWin64FrameworkDependent)
		.Executes();

	[SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "MvdO: Invoked manually.")]
	private Target PublishRelease => _ => _
		.DependsOn(PublishLinux64)
		.DependsOn(PublishWin64)
		.Triggers(CreateScoopManifest)
		.Triggers(CreateWinGetManifest)
		.Triggers(CreateGitHubRelease)
		.Executes();
}