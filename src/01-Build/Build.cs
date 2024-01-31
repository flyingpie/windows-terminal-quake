using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NerdbankGitVersioning;
using Serilog;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
	"continuous",
	GitHubActionsImage.WindowsLatest,
	On = new[] { GitHubActionsTrigger.Push },
	InvokedTargets = new[] { nameof(PublishAll) })]
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
	readonly NerdbankGitVersioning NerdbankVersioning;

	private AbsolutePath ArtifactsDirectory => RootDirectory / "_output" / "artifacts";

	private AbsolutePath StagingDirectory => RootDirectory / "_output" / "staging";

	//private AbsolutePath BuildDirectory => RootDirectory / "build";

	//	AbsolutePath TestResultsDirectory => RootDirectory / "TestResults";

	[Solution(GenerateProjects = true)]
	private readonly Solution Solution;

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

	private Target PublishNet8Win64FrameworkDependent => _ => _
		.DependsOn(Clean)
		.Executes(() =>
		{
			Log.Information("NerdbankVersioning = {Value}", NerdbankVersioning.SimpleVersion);

			var st = StagingDirectory / "net8.0-win_framework-dependent";
			var pub = ArtifactsDirectory / "net8.0-win_framework-dependent.zip";

//			var dir = ArtifactsDirectory / "net8.0-win_framework-dependent";
//			var pub = ArtifactsDirectory / "pub";

			DotNetPublish(_ => _
				//.SetBinaryLog("msbuild.binlog")
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Windows)
				.SetOutput(st)
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(false));

			st.ZipTo(
				pub,
				filter: x => x.HasExtension(".exe", ".jsonc"),
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: FileMode.CreateNew);
		});

	private Target PublishNet8Win64SelfContained => _ => _
		.DependsOn(Restore)
		.Executes(() =>
		{
			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net8.0-windows")
				.SetProject(Solution._0_Host.Wtq_Windows)
				.SetOutput(ArtifactsDirectory / "net8.0-win_self-contained")
				.SetPublishSingleFile(true)
				.SetRuntime("win-x64")
				.SetSelfContained(true));
		});

	private Target PublishAll => _ => _
		.DependsOn(PublishNet8Win64FrameworkDependent)
		//.DependsOn(PublishNet8Win64SelfContained)
		.Executes(() =>
		{
		});
}