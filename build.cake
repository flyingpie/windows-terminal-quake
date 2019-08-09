#tool "nuget:?package=vswhere&version=2.6.7"

var configuration = Argument("configuration", "Release");

var sln = "windows-terminal-quake.sln";

Task("Default").Does(() =>
{
    NuGetRestore(sln);

    MSBuild(sln, new MSBuildSettings
    {
        Configuration = "Release",
        Restore = true,
        ToolPath = GetFiles(VSWhereLatest() + "/**/MSBuild.exe").FirstOrDefault()
    });
});

RunTarget("Default");