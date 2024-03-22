using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NerdbankGitVersioning;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
	"continuous",
	GitHubActionsImage.WindowsLatest,
	FetchDepth = 0,
	On = [GitHubActionsTrigger.Push],
	InvokedTargets = [nameof(PublishAll)])]
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces", Justification = "MvdO: Build script.")]
public sealed class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.PublishAll);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Required]
	[GitRepository]
	private readonly GitRepository GitRepository;

	[NerdbankGitVersioning]
	private readonly NerdbankGitVersioning NerdbankVersioning;

	private AbsolutePath OutputDirectory => RootDirectory / "_output";

	private AbsolutePath ArtifactsDirectory => OutputDirectory / "artifacts";

	private AbsolutePath StagingDirectory => OutputDirectory / "staging";

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	private readonly Solution Solution;

	private AbsolutePath PathToWin64FrameworkDependentZip => ArtifactsDirectory / $"win-x64_framework-dependent.zip";

	private AbsolutePath PathToWin64SelfContainedZip => ArtifactsDirectory / $"win-x64_self-contained.zip";

	private AbsolutePath PathToWinGetManifestDir => ArtifactsDirectory / $"winget";

	private AbsolutePath PathToScoopManifestDir => ArtifactsDirectory / $"scoop";

	private AbsolutePath PathToPsOneLinerDir => ArtifactsDirectory / $"ps-one-liner";

	private GitHubActions GitHubActions => GitHubActions.Instance;

	private Target Clean => _ => _
		.Executes(() =>
		{
			OutputDirectory.CreateOrCleanDirectory();
		});

	private Target Restore => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
		});

	private Target PublishWin64FrameworkDependent => _ => _
		.DependsOn(Clean)
		.Produces(PathToWin64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Windows)
				.SetOutput(st)
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(false));

			st.ZipTo(
				PathToWin64FrameworkDependentZip,
				filter: x => x.HasExtension(".exe", ".jsonc"),
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: FileMode.CreateNew);
		});

	private Target PublishWin64SelfContained => _ => _
		.DependsOn(Restore)
		.Produces(PathToWin64SelfContainedZip)
		.Executes(() =>
		{
			var staging = StagingDirectory / "win-x64_self-contained";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Windows)
				.SetOutput(staging)
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(true));

			staging.ZipTo(
				PathToWin64SelfContainedZip,
				filter: x => x.HasExtension(".exe", ".jsonc"),
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: FileMode.CreateNew);
		});

	private Target CreatePowerShellOneLiner => _ => _
		.Executes(() =>
		{
			// TODO: PowerShell install one-liner
		});

	private Target CreateScoopManifest => _ => _
		.Executes(() =>
		{
			// TODO: Scoop manifest
		});

	private Target CreateWinGetManifest => _ => _
		.Executes(() =>
		{
			// TODO: WinGet manifest
		});

	private Target PublishAll => _ => _
		.DependsOn(PublishWin64FrameworkDependent)
		.DependsOn(PublishWin64SelfContained)
		.Triggers(CreatePowerShellOneLiner)
		.Triggers(CreateScoopManifest)
		.Triggers(CreateWinGetManifest)
		.Executes();
}