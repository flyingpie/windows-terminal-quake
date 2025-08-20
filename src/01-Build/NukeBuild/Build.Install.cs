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
			Log.Information($"Writing wtq.desktop to path '{InstallConstants.Linux.PathToDesktopFile}'");
			File.WriteAllText(InstallConstants.Linux.PathToDesktopFile, InstallConstants.Linux.DesktopFile);

			// Copy binaries
			if (Directory.Exists(InstallConstants.Linux.PathToInstall))
			{
				Log.Warning($"Directory at path '{InstallConstants.Linux.PathToInstall}' already exists, deleting");
				Directory.Delete(InstallConstants.Linux.PathToInstall, recursive: true);
			}

			Log.Information($"Installing WTQ binaries to path '{InstallConstants.Linux.PathToInstall}'");
			Directory.Move(PathToLinux64SelfContained, InstallConstants.Linux.PathToInstall);
		});

	private Target UninstallLinux => _ => _
		.Description($"Uninstalls WTQ, as installed by '{nameof(InstallLinux)}'")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		.Executes(() =>
		{
			// Delete wtq.desktop file
			Log.Information($"Deleting wtq.desktop at path '{InstallConstants.Linux.PathToDesktopFile}'");
			if (File.Exists(InstallConstants.Linux.PathToDesktopFile))
			{
				File.Delete(InstallConstants.Linux.PathToDesktopFile);
			}
			else
			{
				Log.Warning($"Desktop file at path '{InstallConstants.Linux.PathToDesktopFile}' doesn't exist");
			}

			// Delete binaries
			Log.Information($"Deleting WTQ binaries at path '{InstallConstants.Linux.PathToInstall}'");
			if (Directory.Exists(InstallConstants.Linux.PathToInstall))
			{
				Directory.Delete(InstallConstants.Linux.PathToInstall, recursive: true);
			}
			else
			{
				Log.Warning($"Binaries at path '{InstallConstants.Linux.PathToInstall}' don't exist");
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
			Log.Information($"Writing shortcut to path '{InstallConstants.Windows.PathToShortcut}'");

			var link = (IShellLink)new ShellLink();
			link.SetDescription("Enable Quake-mode for (almost) any app");
			link.SetPath(Path.Combine(InstallConstants.Windows.PathToInstall, "wtq.exe"));
			((IPersistFile)link).Save(InstallConstants.Windows.PathToShortcut, false);

			// Copy binaries
			if (Directory.Exists(InstallConstants.Windows.PathToInstall))
			{
				Log.Warning($"Directory at path '{InstallConstants.Windows.PathToInstall}' already exists, deleting");
				Directory.Delete(InstallConstants.Windows.PathToInstall, recursive: true);
			}

			Log.Information($"Installing WTQ binaries to path '{InstallConstants.Windows.PathToInstall}'");
			Directory.Move(PathToWin64SelfContained, InstallConstants.Windows.PathToInstall);
		});

	private Target UninstallWindows => _ => _
		.Description($"Uninstalls WTQ, as installed by '{nameof(UninstallWindows)}'")
		.OnlyWhenStatic(() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		.Executes(() =>
		{
			// Delete shortcut
			Log.Information($"Deleting shortcut at path '{InstallConstants.Windows.PathToShortcut}'");
			if (File.Exists(InstallConstants.Windows.PathToShortcut))
			{
				File.Delete(InstallConstants.Windows.PathToShortcut);
			}
			else
			{
				Log.Warning($"Shortcut at path '{InstallConstants.Windows.PathToShortcut}' doesn't exist");
			}

			// Delete binaries
			Log.Information($"Deleting WTQ binaries at path '{InstallConstants.Windows.PathToInstall}'");
			if (Directory.Exists(InstallConstants.Windows.PathToInstall))
			{
				Directory.Delete(InstallConstants.Windows.PathToInstall, recursive: true);
			}
			else
			{
				Log.Warning($"Binaries at path '{InstallConstants.Windows.PathToInstall}' don't exist");
			}
		});

	#endregion
}