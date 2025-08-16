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
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

public partial class Build
{
	private Target Install => _ => _
		.Description($"Calls either '{nameof(InstallLinux)}' or '{nameof(InstallWindows)}', depending on the current OS")
		.DependsOn(InstallLinux)
		.DependsOn(InstallWindows);

	private Target Uninstall => _ => _
		.Description($"Calls either '{nameof(UninstallLinux)}' or '{nameof(UninstallWindows)}', depending on the current OS")
		.DependsOn(UninstallLinux)
		.DependsOn(UninstallWindows);

	#region Linux

	private Target InstallLinux => _ => _
		.Description("Builds Linux version and installs to ~/.local/share/wtq, includes a .desktop file (respects XDG_DATA_HOME).")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		.DependsOn(BuildLinuxSelfContained)
		.Executes(() =>
		{
			// Write wtq.desktop file
			Log.Information($"Writing wtq.desktop to path '{InstallProps.Linux.PathToDesktopFile}'");
			File.WriteAllText(InstallProps.Linux.PathToDesktopFile, InstallProps.Linux.DesktopFile);

			// Copy binaries
			if (Directory.Exists(InstallProps.Linux.PathToInstall))
			{
				Log.Warning($"Directory at path '{InstallProps.Linux.PathToInstall}' already exists, deleting");
				Directory.Delete(InstallProps.Linux.PathToInstall, recursive: true);
			}

			Log.Information($"Installing WTQ binaries to path '{InstallProps.Linux.PathToInstall}'");
			Directory.Move(PathToLinux64SelfContained, InstallProps.Linux.PathToInstall);
		});

	private Target UninstallLinux => _ => _
		.Description($"Uninstalls WTQ, as installed by '{nameof(InstallLinux)}'")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		.Executes(() =>
		{
			// Delete wtq.desktop file
			Log.Information($"Deleting wtq.desktop at path '{InstallProps.Linux.PathToDesktopFile}'");
			if (File.Exists(InstallProps.Linux.PathToDesktopFile))
			{
				File.Delete(InstallProps.Linux.PathToDesktopFile);
			}
			else
			{
				Log.Warning($"Desktop file at path '{InstallProps.Linux.PathToDesktopFile}' doesn't exist");
			}

			// Delete binaries
			Log.Information($"Deleting WTQ binaries at path '{InstallProps.Linux.PathToInstall}'");
			if (Directory.Exists(InstallProps.Linux.PathToInstall))
			{
				Directory.Delete(InstallProps.Linux.PathToInstall, recursive: true);
			}
			else
			{
				Log.Warning($"Binaries at path '{InstallProps.Linux.PathToInstall}' don't exist");
			}
		});

	#endregion

	#region Windows

	private Target InstallWindows => _ => _
		.Description("Builds Windows version and installs to %APPDATA%/wtq, includes a start menu shortcut.")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		.DependsOn(BuildWindows64SelfContained)
		.Executes(() =>
		{
			// Write shortcut
			Log.Information($"Writing shortcut to path '{InstallProps.Windows.PathToShortcut}'");

			var link = (IShellLink)new ShellLink();
			link.SetDescription("Enable Quake-mode for (almost) any app");
			link.SetPath(Path.Combine(InstallProps.Windows.PathToInstall, "wtq.exe"));
			((IPersistFile)link).Save(InstallProps.Windows.PathToShortcut, false);

			// Copy binaries
			if (Directory.Exists(InstallProps.Windows.PathToInstall))
			{
				Log.Warning($"Directory at path '{InstallProps.Windows.PathToInstall}' already exists, deleting");
				Directory.Delete(InstallProps.Windows.PathToInstall, recursive: true);
			}

			Log.Information($"Installing WTQ binaries to path '{InstallProps.Windows.PathToInstall}'");
			Directory.Move(PathToWin64SelfContained, InstallProps.Windows.PathToInstall);
		});

	private Target UninstallWindows => _ => _
		.Description($"Uninstalls WTQ, as installed by '{nameof(UninstallWindows)}'")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		.Executes(() =>
		{
			// Delete shortcut
			Log.Information($"Deleting shortcut at path '{InstallProps.Windows.PathToShortcut}'");
			if (File.Exists(InstallProps.Windows.PathToShortcut))
			{
				File.Delete(InstallProps.Windows.PathToShortcut);
			}
			else
			{
				Log.Warning($"Shortcut at path '{InstallProps.Windows.PathToShortcut}' doesn't exist");
			}

			// Delete binaries
			Log.Information($"Deleting WTQ binaries at path '{InstallProps.Windows.PathToInstall}'");
			if (Directory.Exists(InstallProps.Windows.PathToInstall))
			{
				Directory.Delete(InstallProps.Windows.PathToInstall, recursive: true);
			}
			else
			{
				Log.Warning($"Binaries at path '{InstallProps.Windows.PathToInstall}' don't exist");
			}
		});

	#endregion
}