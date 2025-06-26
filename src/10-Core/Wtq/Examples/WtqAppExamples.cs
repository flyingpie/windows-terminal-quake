using System.Runtime.InteropServices;

namespace Wtq.Examples;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "MVdO: Reference data.")]
public static class WtqAppExamples
{
	public static class Categories
	{
		public const string Browsers = "Browsers";
		public const string FileManagers = "File Managers";
		public const string KeysAndPasswords = "Keys & Passwords";
		public const string Media = "Media";
		public const string Misc = "Misc";
		public const string Monitoring = "Monitoring";
		public const string NoteTaking = "Note Taking";
		public const string Social = "Social";
		public const string Terminals = "Terminals";
	}

	public static class Flavors
	{
		public const string Flatpak = "Flatpak";
		public const string Native = "Native";
	}

	public static ICollection<WtqAppExample> AppExamples { get; } =
	[
		#region Browsers

		// Chrome
		// TODO

		// Chromium
		// TODO

		// Edge
		// TODO

		// Firefox
		// TODO

		#endregion

		#region File Managers

		// Dolphin
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Dolphin",
			Description = "The default file manager for KDE.",
			Image = "/example_apps/dolphin.jpg",
			Link = new("https://apps.kde.org/dolphin/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "dolphin",
				}
			],
		},

		// Double Commander
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Double Commander",
			Description = "Midnight Commander-inspired file manager.",
			Image = "/example_apps/double_commander.png",
			Link = new("https://doublecommander.com"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "TODO",
				},
			],
		},

		// Q-Dir
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Q-Dir",
			Description = "File manager with support for multiple panes, featuring lots of customization.",
			Image = "/example_apps/q_dir.png",
			Link = new("http://q-dir.com/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "TODO",
				},
			],
		},

		// Windows Explorer
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Windows Explorer",
			Description = "Windows explorer, the default file browser on Windows.",
			Image = "/example_apps/windows_explorer.png",
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
					FileName = "explorer",
				},
			],
		},

		#endregion

		#region Keys & Passwords

		// Bitwarden

		// KeePassXC
		new()
		{
			Categories = [Categories.Misc],
			Title = "KeePassXC",
			Description = "Password manager.",
			Image = "/example_apps/keepassxc.png",
			Link = new("https://keepassxc.org/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux, OSPlatform.Windows,],
					FileName = "keepassxc",
				},
			],
		},

		// Proton Pass
		// TODO

		#endregion

		#region Media

		// AudioTube
		// TODO
		// https://flathub.org/apps/org.kde.audiotube

		// Spotify
		new()
		{
			Categories = [Categories.Media],
			Title = "Spotify",
			Description = "Music streaming service.",
			Image = "/example_apps/spotify.jpg",
			Link = new("https://www.spotify.com/us/download/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux, OSPlatform.Windows,],
					FileName = "spotify",
				},
			],
		},

		#endregion

		#region Misc

		// Dev Tools

		// Dev Toolbox
		// TODO
		// https://flathub.org/apps/me.iepure.devtoolbox

		// DevToys
		// TODO
		// https://devtoys.app/

		// Bruno
		// TODO

		// Insomnia
		// TODO

		// Postman
		// TODO

		// Yaak
		// TODO

		// /Dev Tools

		// Editors

		// Kate
		// TODO

		// Sublime Text
		// TODO

		// VSCode
		// TODO

		// /Editors

		// File Sharing

		// LocalSend
		// TODO

		// Warpinator
		// TODO

		// /File Sharing

		#endregion

		#region Monitoring

		// Mission Center
		// TODO
		// https://flathub.org/apps/io.missioncenter.MissionCenter

		// Process Explorer
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "Process Explorer",
			Description = "Task manager on steroids.",
			Image = "/example_apps/process_explorer.jpg",
			Link = new("https://learn.microsoft.com/en-us/sysinternals/downloads/process-explorer"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "procexp64",
				},
			],
		},

		// Resources
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "Resources",
			Description = "Gnome libadwaita-based tool for monitoring the system.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://nokyan.net/projects/resources/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "resources",
				},
			],
		},

		// System Informer
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "System Informer",
			Description = "Task manager on even more steroids.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://github.com/winsiderss/systeminformer"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "systeminformer",
				},
			],
		},

		// System Monitor (KDE)
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "System Monitor",
			Description = "Default task manager for KDE.",
			Image = "/example_apps/system_monitor.jpg",
			Link = new("https://apps.kde.org/plasma-systemmonitor/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "plasma-systemmonitor",
				},
			],
		},

		#endregion

		#region Note Taking

		// Joplin
		// TODO

		// Logseq
		// TODO

		// Notion
		// TODO

		// Obsidian
		// TODO

		// Rnote
		// TODO

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
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "discord",
				},
			],
		},

		// Element (Matrix Client)
		// TODO
		// https://flathub.org/apps/im.riot.Riot

		// Signal
		// TODO

		// Telegram
		// TODO

		// Vesktop
		// TODO

		// WhatsApp
		new()
		{
			Categories = [Categories.Social],
			Title = "WhatsApp",
			Description = "Chat service.",
			Image = "/example_apps/placeholder.png",
			Link = new("https://whatsapp.com/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "whatsapp",
				},
			],
		},

		#endregion

		#region Terminal & Shells

		// Alacritty
		// TODO

		// Ghostty
		// TODO

		// Kitty
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Kitty",
			Description = "Cross-platform, fast, feature-rich, GPU based terminal.",
			Image = "/example_apps/kitty.png",
			Link = new("https://sw.kovidgoyal.net/kitty/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux, OSPlatform.Windows,],
					FileName = "kitty",
				},
			],
		},

		// Konsole
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Konsole",
			Description = "The default console emulator for KDE.",
			Image = "/example_apps/konsole.png",
			Link = new("https://konsole.kde.org/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "konsole",
				},
			],
		},

		// PowerShell <=5 (Windows-only)
		new()
		{
			Categories = [Categories.Terminals],
			Title = "PowerShell 5",
			Description = "PowerShell",
			Image = "/example_apps/powershell_5.jpg",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-5.1"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "powershell",
				},
			],
		},

		// PowerShell >=6 (Cross-platform)
		new()
		{
			Categories = [Categories.Terminals],
			Title = "PowerShell Core",
			Description = "The cross-platform PowerShell, (version 6 and up, a.k.a. 'PowerShell Core').",
			Image = "/example_apps/powershell_core.png",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4"),
			Flavors =
			[
				new()
				{
					Name = "PowerShell 7",
					Os = [OSPlatform.Windows,],
					FileName = "pwsh",
				},
			],
		},

		// Ptyxis
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Ptyxis",
			Description = "Ptyxis is a terminal for GNOME that focuses on ease-of-use in a world of containers.",
			Image = "/example_apps/ptyxis.webp",
			Link = new("https://gitlab.gnome.org/chergert/ptyxis"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "ptyxis",
				},
			],
		},

		// Wezterm
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Wezterm",
			Description = "Cross-platform GPU-accelerated terminal emulator.",
			Image = "/example_apps/wezterm.png",
			Link = new("https://wezfurlong.org/wezterm/index.html"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux, OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
					FileName = "wezterm",
				},
			],
		},

		// Windows Terminal
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Windows Terminal",
			Description = "Windows Terminal",
			Image = "/example_apps/windows_terminal.jpg",
			Link = new("https://learn.microsoft.com/en-us/windows/terminal/install"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					FileName = "wt",
					Factory = e => new()
					{
						Name = e.Title,
						FileName = "wt",
						ProcessName = "WindowsTerminal",
					},
				},
			],
		},

		#endregion
	];
}