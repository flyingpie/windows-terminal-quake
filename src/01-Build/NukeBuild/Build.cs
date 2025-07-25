#pragma warning disable
// ReSharper disable All

using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Serilog;
using System;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
	"ci",
	GitHubActionsImage.Ubuntu2204, // Note that on later versions, .Net 9 is not (yet) available.
	FetchDepth = 0,
	OnPushBranches = ["master"],
	OnWorkflowDispatchOptionalInputs = ["name"],
	EnableGitHubToken = true,
	InvokedTargets = [nameof(Publish)])]
public sealed partial class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.BuildAll);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = Configuration.Release;

	[Parameter("GitHubRelease")]
	private string GitHubRelease = "vNext";

	[Parameter("GitHub Token")]
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

	private AbsolutePath PathToLinux64FrameworkDependentZipSha256 => ArtifactsDirectory / "linux-x64_framework-dependent.tar.gz.sha256";

	private AbsolutePath PathToLinux64SelfContainedZip => ArtifactsDirectory / "linux-x64_self-contained.tar.gz";

	private AbsolutePath PathToLinux64SelfContainedZipSha256 => ArtifactsDirectory / "linux-x64_self-contained.tar.gz.sha256";

	private AbsolutePath PathToWin64FrameworkDependentZip => ArtifactsDirectory / "win-x64_framework-dependent.zip";

	private AbsolutePath PathToWin64FrameworkDependentZipSha256 => ArtifactsDirectory / "win-x64_framework-dependent.zip.sha256";

	private AbsolutePath PathToWin64SelfContainedZip => ArtifactsDirectory / "win-x64_self-contained.zip";

	private AbsolutePath PathToWin64SelfContainedZipSha256 => ArtifactsDirectory / "win-x64_self-contained.zip.sha256";

	private GitHubActions GitHubActions => GitHubActions.Instance;

	private string SemVerVersion => XmlTasks.XmlPeekSingle(RootDirectory / "src" / "Directory.Build.props", "//Version");

	private Target Info => _ => _
		.Description("Show build settings and output paths.")
		.Executes(() =>
		{
			Log.Information("Configuration:.........{Configuration}", Configuration);

			Log.Information("GitHubRelease:.........{GitHubRelease}", GitHubRelease);
			Log.Information("SemVerVersion:.........{SemVerVersion}", SemVerVersion);

			Log.Information("OutputDirectory:.......{OutputDirectory}", OutputDirectory);
			Log.Information("StagingDirectory:......{StagingDirectory}", StagingDirectory);
			Log.Information("ArtifactsDirectory:....{ArtifactsDirectory}", ArtifactsDirectory);
		});

	/// <summary>
	/// Clean output directories.
	/// </summary>
	private Target Clean => _ => _
		.Description("Delete output folders.")
		.DependsOn(Info)
		.Executes(() => OutputDirectory.CreateOrCleanDirectory());

	/// <summary>
	/// Tests.
	/// </summary>
	private Target RunTests => _ => _
		.Description("Run all unit tests.")
		.DependsOn(Clean)
		.Executes(() =>
		{
			DotNetTest(_ => _
				.SetProjectFile(Solution.Path)
				.SetVerbosity(DotNetVerbosity.normal)
			);
		});

	private Target BuildQuick => _ => _
		.Description("Build everything, skip tests.")
		.DependsOn(Clean)
		.DependsOn(BuildLinux)
		.DependsOn(BuildWindows)
		.Executes();

	private Target BuildAll => _ => _
		.Description("Build everything.")
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.DependsOn(BuildLinux)
		.DependsOn(BuildWindows)
		.Executes();

	private Target Publish => _ => _
		.Description("Build everything and publish to GitHub.")
		.DependsOn(BuildAll)
		.Triggers(CreateScoopManifest)
		.Triggers(CreateWinGetManifest)
		.Triggers(CreateGitHubRelease)
		.Executes();
}