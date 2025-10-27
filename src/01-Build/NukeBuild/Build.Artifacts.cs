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
		.DependsOn(Restore)
		.Produces(PathToLinux64FrameworkDependentZip)
		.Produces(PathToLinux64FrameworkDependentZipSha256)
		.Executes(() =>
		{
			var st = PathToLinux64FrameworkDependent;

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st));

			st.TarGZipTo(PathToLinux64FrameworkDependentZip, fileMode: FileMode.CreateNew);

			PathToLinux64FrameworkDependentZipSha256.WriteAllText(PathToLinux64FrameworkDependentZip.GetFileHashSha256());
		});

	/// <summary>
	/// Linux x64 self-contained.
	/// </summary>
	private Target BuildLinuxSelfContained => _ => _
		.DependsOn(Restore)
		.Produces(PathToLinux64SelfContainedZip)
		.Produces(PathToLinux64SelfContainedZipSha256)
		.Executes(() =>
		{
			var st = PathToLinux64SelfContained;

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetProject(Solution._0_Host.Wtq_Host_Linux)
				.SetOutput(st)
				.SetRuntime("linux-x64")
				.SetSelfContained(true));

			st.TarGZipTo(PathToLinux64SelfContainedZip, fileMode: FileMode.CreateNew);

			PathToLinux64SelfContainedZipSha256.WriteAllText(PathToLinux64SelfContainedZip.GetFileHashSha256());
		});

	/// <summary>
	/// Windows x64 framework dependent.
	/// </summary>
	private Target BuildWindowsFrameworkDependent => _ => _
		.DependsOn(Restore)
		.Produces(PathToWin64FrameworkDependentZip)
		.Produces(PathToWin64FrameworkDependentZipSha256)
		.Executes(() =>
		{
			var st = PathToWin64FrameworkDependent;

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetSelfContained(false));

			st.ZipTo(PathToWin64FrameworkDependentZip, compressionLevel: CompressionLevel.SmallestSize, fileMode: FileMode.CreateNew);

			PathToWin64FrameworkDependentZipSha256.WriteAllText(PathToWin64FrameworkDependentZip.GetFileHashSha256());
		});

	/// <summary>
	/// Windows x64 self-contained.
	/// </summary>
	private Target BuildWindows64SelfContained => _ => _
		.DependsOn(Restore)
		.Produces(PathToWin64SelfContainedZip)
		.Produces(PathToWin64SelfContainedZipSha256)
		.Executes(() =>
		{
			var st = PathToWin64SelfContained;

			DotNetPublish(_ => _
				.SetConfiguration(Configuration)
				.SetFramework("net9.0-windows")
				.SetProject(Solution._0_Host.Wtq_Host_Windows)
				.SetOutput(st)
				.SetRuntime("win-x64")
				.SetSelfContained(true));

			st.ZipTo(PathToWin64SelfContainedZip, compressionLevel: CompressionLevel.SmallestSize, fileMode: FileMode.CreateNew);

			PathToWin64SelfContainedZipSha256.WriteAllText(PathToWin64SelfContainedZip.GetFileHashSha256());
		});
}