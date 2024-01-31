using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NerdbankGitVersioning;
using System.IO;
using System.IO.Compression;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
	"continuous",
	GitHubActionsImage.WindowsLatest,
	FetchDepth = 0,
	On = [GitHubActionsTrigger.Push],
	InvokedTargets = [nameof(PublishAll)])]
public sealed class Build : NukeBuild
{
	public static int Main() => Execute<Build>(x => x.PublishAll);

	[Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
	private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

	[Required]
	[GitRepository]
	private readonly GitRepository GitRepository;

	//[GitVersion]
	//readonly GitVersion GitVersion;

	[NerdbankGitVersioning]
	private readonly NerdbankGitVersioning NerdbankVersioning;

	private AbsolutePath ArtifactsDirectory => RootDirectory / "_output" / "artifacts";

	private AbsolutePath StagingDirectory => RootDirectory / "_output" / "staging";

	//private AbsolutePath BuildDirectory => RootDirectory / "build";

	//	AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";

	private GitHubActions GitHubActions => GitHubActions.Instance;

	[Solution(GenerateProjects = true, SuppressBuildProjectCheck = true)]
	private readonly Solution Solution;

	private AbsolutePath PathToWin64FrameworkDependentZip => ArtifactsDirectory / $"{NerdbankVersioning?.NuGetPackageVersion}-win-x64_framework-dependent.zip";

	private AbsolutePath PathToWin64SelfContainedZip => ArtifactsDirectory / $"{NerdbankVersioning?.NuGetPackageVersion}-win-x64_self-contained.zip";

	private Target Clean => _ => _
		.Executes(() =>
		{
			ArtifactsDirectory.CreateOrCleanDirectory();
			//BuildDirectory.CreateOrCleanDirectory();
		});

	private Target Restore => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
		});

	//private Target Compile => _ => _
	//	.DependsOn(Restore)
	//	.Executes(() =>
	//	{
	//		DotNetBuild(_ => _
	//			.SetBinaryLog("msbuild.binlog")
	//			.SetConfiguration(Configuration)
	//			.SetFramework("net8.0-windows")
	//			.ProjectFile(Solution._0_Host.Wtq_Windows)
	//			.SetOutput(ArtifactsDirectory / "net8.0-windows_framework-dependent")
	//			.SetPublishSingleFile(true)
	//			.SetRuntime("win-x64")
	//			.SetPublishReadyToRun(true)
	//			.SetSelfContained(false));
	//	});

	private Target PublishWin64FrameworkDependent => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
			var st = StagingDirectory / $"{NerdbankVersioning.NuGetPackageVersion}-win-x64_framework-dependent";

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

	private Target PublishAll => _ => _
		.DependsOn(PublishWin64FrameworkDependent)
		.DependsOn(PublishWin64SelfContained)
		.Produces(
			PathToWin64FrameworkDependentZip,
			PathToWin64SelfContainedZip)
		.Executes();
}