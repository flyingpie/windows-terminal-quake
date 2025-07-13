#pragma warning disable
// ReSharper disable All

using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using System.IO.Compression;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using FileMode = System.IO.FileMode;

/// <summary>
/// Artifact-producing targets.
/// </summary>
public sealed partial class Build
{
	private Target BuildLinux => _ => _
		.DependsOn(BuildLinuxFrameworkDependent)
		.DependsOn(BuildLinuxSelfContained);

	private Target BuildWindows => _ => _
		.DependsOn(BuildWindowsFrameworkDependent)
		.DependsOn(BuildWindows64SelfContained);

	/// <summary>
	/// Linux x64 framework dependent.
	/// </summary>
	private Target BuildLinuxFrameworkDependent => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToLinux64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "linux-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st));

			st.TarGZipTo(
				PathToLinux64FrameworkDependentZip,
				fileMode: FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 self-contained.
	/// </summary>
	private Target BuildLinuxSelfContained => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToLinux64SelfContainedZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "linux-x64_self-contained";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st)
				.SetRuntime("linux-x64")
				.SetSelfContained(true));

			st.TarGZipTo(
				PathToLinux64SelfContainedZip,
				fileMode: FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 framework dependent.
	/// </summary>
	private Target BuildWindowsFrameworkDependent => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToWin64FrameworkDependentZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_framework-dependent";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetSelfContained(false));

			st.ZipTo(
				PathToWin64FrameworkDependentZip,
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: FileMode.CreateNew);
		});

	/// <summary>
	/// Windows x64 self-contained.
	/// </summary>
	private Target BuildWindows64SelfContained => _ => _
		.DependsOn(Clean)
		.DependsOn(RunTests)
		.Produces(PathToWin64SelfContainedZip)
		.Executes(() =>
		{
			var st = StagingDirectory / "win-x64_self-contained";

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetRuntime("win-x64")
				.SetSelfContained(true));

			st.ZipTo(
				PathToWin64SelfContainedZip,
				compressionLevel: CompressionLevel.SmallestSize,
				fileMode: FileMode.CreateNew);
		});
}