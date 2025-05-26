using System.Runtime.InteropServices;

namespace Wtq;

public class WtqAppExamples
{
	public static class Categories
	{
		public const string FileManagers = "File Managers";
		public const string Media = "Media";
		public const string Misc = "Misc";
		public const string Monitoring = "Monitoring";
		public const string Social = "Social";
		public const string TerminalsAndShells = "Terminals & Shells";
	}

	public static ICollection<WtqAppExample> AppExamples { get; } =
	[
		#region File Managers

		// Windows Explorer
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Windows Explorer",
			Description = "Windows explorer, the default file browser on Windows.",
			Image = "/example_apps/windows_explorer.png",
			Os = [OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
			FileName = "explorer",
		},

		// Double Commander
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Double Commander",
			Description = "Midnight Commander-inspired file manager.",
			Image = "/example_apps/double_commander.png",
			Link = new("https://doublecommander.com"),
			Os = [OSPlatform.Windows,],
			FileName = "TODO",
		},

		// Q-Dir
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Q-Dir",
			Description = "File manager with support for multiple panes, featuring lots of customization.",
			Image = "/example_apps/q_dir.png",
			Link = new("http://q-dir.com/"),
			Os = [OSPlatform.Windows,],
			FileName = "TODO",
		},

		// Dolphin
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Dolphin",
			Description = "The default file manager for KDE.",
			Image = "/example_apps/dolphin.jpg",
			Link = new("https://apps.kde.org/dolphin/"),
			Os = [OSPlatform.Linux,],
			FileName = "dolphin",
		},

		#endregion

		#region Media

		// Spotify
		new()
		{
			Categories = [Categories.Media],
			Title = "Spotify",
			Description = "Music streaming service.",
			Image = "/example_apps/spotify.jpg",
			Link = new("https://www.spotify.com/us/download/"),
			Os = [OSPlatform.Linux, OSPlatform.Windows,],
			FileName = "spotify",
		},

		#endregion

		#region Misc

		// KeePassXC
		new()
		{
			Categories = [Categories.Misc],
			Title = "KeePassXC",
			Description = "Password manager.",
			Image = "/example_apps/keepassxc.png",
			Link = new("https://keepassxc.org/"),
			Os = [OSPlatform.Linux, OSPlatform.Windows,],
			FileName = "keepassxc",
		},

		#endregion

		#region Monitoring

		// Process Explorer
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "Process Explorer",
			Description = "Task manager on steroids.",
			Image = "/example_apps/process_explorer.jpg",
			Link = new("https://learn.microsoft.com/en-us/sysinternals/downloads/process-explorer"),
			Os = [OSPlatform.Windows,],
			FileName = "procexp64",
		},

		// Resources
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "Resources",
			Description = "Gnome libadwaita-based tool for monitoring the system.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://nokyan.net/projects/resources/"),
			Os = [OSPlatform.Linux,],
			FileName = "resources",
		},

		// System Informer
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "System Informer",
			Description = "Task manager on even more steroids.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://github.com/winsiderss/systeminformer"),
			Os = [OSPlatform.Windows,],
			FileName = "systeminformer",
		},

		// System Monitor (KDE)
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "System Monitor",
			Description = "Default task manager for KDE.",
			Image = "/example_apps/system_monitor.jpg",
			Link = new("https://apps.kde.org/plasma-systemmonitor/"),
			Os = [OSPlatform.Linux,],
			FileName = "plasma-systemmonitor",
		},

		#endregion

		#region Social

		// Discord
		new()
		{
			Categories = [Categories.Social],
			Title = "Discord",
			Description = "Chat service.",
			Image = "/example_apps/discord.png",
			Link = new("https://discord.com/"),
			Os = [OSPlatform.Windows,],
			FileName = "discord",
		},

		// WhatsApp
		new()
		{
			Categories = [Categories.Social],
			Title = "WhatsApp",
			Description = "Chat service.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://whatsapp.com/"),
			Os = [OSPlatform.Windows,],
			FileName = "whatsapp",
		},

		#endregion

		#region Terminal Emulators & Shells

		// Windows Terminal
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "Windows Terminal",
			Description = "Windows Terminal",
			Image = "/example_apps/windows_terminal.jpg",
			Link = new("https://learn.microsoft.com/en-us/windows/terminal/install"),
			Os = [OSPlatform.Windows,],
			FileName = "wt",
			Factory = () => new()
			{
				Name = "Windows Terminal",
				FileName = "wt",
				ProcessName = "WindowsTerminal",
			},
		},

		// PowerShell <=5 (Windows-only)
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "PowerShell 5",
			Description = "The original PowerShell, (Windows-only).",
			Image = "/example_apps/powershell_5.jpg",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-5.1"),
			Os = [OSPlatform.Windows,],
			FileName = "powershell",
		},

		// PowerShell >=6 (Cross-platform)
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "PowerShell Core",
			Description = "The cross-platform PowerShell, (version 6 and up, a.k.a. 'PowerShell Core').",
			Image = "/example_apps/powershell_core.png",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4"),
			Os = [OSPlatform.Windows,],
			FileName = "pwsh",
		},

		// Wezterm
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "Wezterm",
			Description = "Cross-platform GPU-accelerated terminal emulator.",
			Image = "/example_apps/wezterm.png",
			Link = new("https://wezfurlong.org/wezterm/index.html"),
			Os = [OSPlatform.Linux, OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
			FileName = "wezterm",
		},

		// Kitty
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "Kitty",
			Description = "Cross-platform, fast, feature-rich, GPU based terminal.",
			Image = "/example_apps/kitty.png",
			Link = new("https://sw.kovidgoyal.net/kitty/"),
			Os = [OSPlatform.Linux, OSPlatform.Windows,],
			FileName = "kitty",
		},

		// Ptyxis
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "Ptyxis",
			Description = "Ptyxis is a terminal for GNOME that focuses on ease-of-use in a world of containers.",
			Image = "/example_apps/ptyxis.webp",
			Link = new("https://gitlab.gnome.org/chergert/ptyxis"),
			Os = [OSPlatform.Linux,],
			FileName = "ptyxis",
		},

		// Konsole
		new()
		{
			Categories = [Categories.TerminalsAndShells],
			Title = "Konsole",
			Description = "The default console emulator for KDE.",
			Image = "/example_apps/konsole.png",
			Link = new("https://konsole.kde.org/"),
			Os = [OSPlatform.Linux,],
			FileName = "konsole",
		},

		#endregion
	];
}