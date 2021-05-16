#tool "nuget:?package=vswhere&version=2.6.7"
#tool "nuget:?package=ILRepack&version=2.0.18"

var configuration = Argument("configuration", "Release");
var output = Argument("output", "artifacts");
var version = Argument("version", "1.2.0");

var sln = "windows-terminal-quake.sln";
var bin = $"./windows-terminal-quake/bin/{configuration}/net472";

Task("Clean")
	.Does(() =>
	{
		CleanDirectory(output);
	});

Task("Build")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		MSBuild(sln, new MSBuildSettings
		{
			Configuration = "Release",
			Restore = true,
			ToolPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault()
		}
			.WithProperty("AssemblyVersion", version)
			.WithProperty("FileVersion", version)
			.WithProperty("InformationalVersion", version)
			.WithProperty("PackageVersion", version)
		);
	});

Task("Artifact.Regular")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var art = output + "/Artifact.Regular";
		CopyDirectory(bin, art);
		DeleteFiles(art + "/*.config");
		DeleteFiles(art + "/*.pdb");
	});

Task("Artifact.SingleExe")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var deps = GetFiles(bin + "/*.dll");
		var art = output + "/Artifact.SingleExe";
		System.IO.Directory.CreateDirectory(art);

		ILRepack(
			art + "/windows-terminal-quake.exe",		// Output file
			bin + "/windows-terminal-quake.exe",		// Primary assembly
			deps,										// Assembly paths
			new ILRepackSettings()
		);

		CopyFile(bin + "/windows-terminal-quake.json", art + "/windows-terminal-quake.json");
		CopyFile(bin + "/windows-terminal-quake.schema.1.json", art + "/windows-terminal-quake.schema.1.json");
		DeleteFile(art + "/windows-terminal-quake.exe.config");
	});

Task("Artifact.SingleExe.Zip")
	.IsDependentOn("Artifact.SingleExe")
	.Does(() =>
	{
		var art = output + "/Artifact.SingleExe.Zip";
		System.IO.Directory.CreateDirectory(art);

		Zip(output + "/Artifact.SingleExe", art + $"/windows-terminal-quake-{version}-{DateTimeOffset.UtcNow:yyyy-MM-dd_HHmm}.zip");
	});

Task("Default")
	.IsDependentOn("Artifact.Regular")
	.IsDependentOn("Artifact.SingleExe")
	.IsDependentOn("Artifact.SingleExe.Zip")
	.Does(() => {});

RunTarget("Default");
