using System.Runtime.InteropServices;

namespace Wtq.Examples;

[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "MVdO: Reference data.")]
public static class WtqAppExamples
{
	public static class Categories
	{
		public const string Browsers = "Browsers";
		public const string DevTools = "Dev Tools";
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
		// new()
		// {
		// 	Categories = [Categories.Browsers],
		// 	Title = "Chrome",
		// 	Description = "TODO",
		// 	Image = "/example-apps/chrome.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Chromium
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Browsers],
		// 	Title = "Chromium",
		// 	Description = "TODO",
		// 	Image = "/example-apps/chromium.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Edge
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Browsers],
		// 	Title = "Edge",
		// 	Description = "TODO",
		// 	Image = "/example-apps/edge.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Firefox
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Browsers],
		// 	Title = "Firefox",
		// 	Description = "TODO",
		// 	Image = "/example-apps/firefox.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		#endregion

		#region Dev Tools

		// Dev Toolbox
		new()
		{
			Categories = [Categories.DevTools],
			Title = "Dev Toolbox",
			Description = "TODO",
			Image = "/example-apps/dev-toolbox.webp",
			Link = new("https://github.com/aleiepure/devtoolbox"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "devtoolbox",
				},
				new()
				{
					Name = Flavors.Flatpak,
					Os = [OSPlatform.Linux,],
					FileName = "me.iepure.devtoolbox",
				}
			],
		},

		// DevToys
		// TODO
		// https://devtoys.app/
		// new()
		// {
		// 	Categories = [Categories.DevTools],
		// 	Title = "DevToys",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Bruno
		new()
		{
			Categories = [Categories.DevTools],
			Title = "Bruno",
			Description = "Fast and Git-Friendly API Client.",
			Image = "/example-apps/bruno.webp",
			Link = new("https://www.usebruno.com/"),
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Linux,],
					FileName = "bruno",
				}
			],
		},

		// Insomnia
		// TODO
		// new()
		// {
		// 	Categories = [Categories.DevTools],
		// 	Title = "Insomnia",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Yaak
		// TODO
		// new()
		// {
		// 	Categories = [Categories.DevTools],
		// 	Title = "Yaak",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		#endregion

		#region File Managers

		// Dolphin
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Dolphin",
			Description = "The default file manager for KDE.",
			Image = "/example-apps/dolphin.webp",
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
		// TODO
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Double Commander (TODO)",
			Description = "Midnight Commander-inspired file manager.",
			Image = "/example-apps/double-commander.webp",
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
		// TODO
		new()
		{
			Categories = [Categories.FileManagers],
			Title = "Q-Dir (TODO)",
			Description = "File manager with support for multiple panes, featuring lots of customization.",
			Image = "/example-apps/q-dir.webp",
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
			Image = "/example-apps/windows-explorer.webp",
			Flavors =
			[
				new()
				{
					Name = Flavors.Native,
					Os = [OSPlatform.Windows,],
					Factory = x => new()
					{
						Name = x.Title,
						FileName = "explorer",
						WindowClass = "CabinetWClass",
					},
				},
			],
		},

		#endregion

		#region Keys & Passwords

		// Bitwarden
		// TODO
		// new()
		// {
		// 	Categories = [Categories.KeysAndPasswords],
		// 	Title = "Bitwarden",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// KeePassXC
		new()
		{
			Categories = [Categories.Misc],
			Title = "KeePassXC",
			Description = "Password manager.",
			Image = "/example-apps/keepassxc.webp",
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
		// new()
		// {
		// 	Categories = [Categories.KeysAndPasswords],
		// 	Title = "Proton Pass",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		#endregion

		#region Media

		// AudioTube
		// TODO
		// // https://flathub.org/apps/org.kde.audiotube
		// new()
		// {
		// 	Categories = [Categories.Media],
		// 	Title = "AudioTube",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Spotify
		new()
		{
			Categories = [Categories.Media],
			Title = "Spotify",
			Description = "Music streaming service.",
			Image = "/example-apps/spotify.webp",
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

		// Editors

		// Kate
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Misc],
		// 	Title = "Kate",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Sublime Text
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Misc],
		// 	Title = "Sublime Text",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// VSCode
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Misc],
		// 	Title = "Visual Studio Code",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// /Editors

		// File Sharing

		// LocalSend
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Misc],
		// 	Title = "LocalSend",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Warpinator
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Misc],
		// 	Title = "Warpinator",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// /File Sharing

		#endregion

		#region Monitoring

		// Mission Center
		// TODO
		// // https://flathub.org/apps/io.missioncenter.MissionCenter
		// new()
		// {
		// 	Categories = [Categories.Monitoring],
		// 	Title = "Mission Center",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Process Explorer
		new()
		{
			Categories = [Categories.Monitoring],
			Title = "Process Explorer",
			Description = "Task manager on steroids.",
			Image = "/example-apps/process-explorer.webp",
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
			Image = "/example-apps/placeholder.webp",
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
			Image = "/example-apps/placeholder.webp",
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
			Image = "/example-apps/system-monitor.webp",
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
		// new()
		// {
		// 	Categories = [Categories.NoteTaking],
		// 	Title = "Joplin",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Logseq
		// TODO
		// new()
		// {
		// 	Categories = [Categories.NoteTaking],
		// 	Title = "Logseq",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Notion
		// TODO
		// new()
		// {
		// 	Categories = [Categories.NoteTaking],
		// 	Title = "Notion",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Obsidian
		// TODO
		// new()
		// {
		// 	Categories = [Categories.NoteTaking],
		// 	Title = "Obsidian",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Rnote
		// TODO
		// new()
		// {
		// 	Categories = [Categories.NoteTaking],
		// 	Title = "Rnote",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		#endregion

		#region Social

		// Discord
		new()
		{
			Categories = [Categories.Social],
			Title = "Discord",
			Description = "Chat service.",
			Image = "/example-apps/discord.webp",
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
		// // https://flathub.org/apps/im.riot.Riot
		// new()
		// {
		// 	Categories = [Categories.Social],
		// 	Title = "Element",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Signal
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Social],
		// 	Title = "Signal",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Telegram
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Social],
		// 	Title = "Telegram",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Vesktop
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Social],
		// 	Title = "Vesktop",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// WhatsApp
		new()
		{
			Categories = [Categories.Social],
			Title = "WhatsApp",
			Description = "Chat service.",
			Image = "/example-apps/placeholder.webp",
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
		// new()
		// {
		// 	Categories = [Categories.Terminals],
		// 	Title = "Alacritty",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Ghostty
		// TODO
		// new()
		// {
		// 	Categories = [Categories.Terminals],
		// 	Title = "Ghostty",
		// 	Description = "TODO",
		// 	Image = "/example-apps/dolphin.webp",
		// 	Link = new("https://apps.kde.org/dolphin/"),
		// 	Flavors =
		// 	[
		// 		new()
		// 		{
		// 			Name = Flavors.Native,
		// 			Os = [OSPlatform.Linux,],
		// 			FileName = "TODO",
		// 		}
		// 	],
		// },

		// Kitty
		new()
		{
			Categories = [Categories.Terminals],
			Title = "Kitty",
			Description = "Cross-platform, fast, feature-rich, GPU based terminal.",
			Image = "/example-apps/kitty.webp",
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
			Image = "/example-apps/konsole.webp",
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
			Image = "/example-apps/powershell-5.webp",
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
			Image = "/example-apps/powershell-core.webp",
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
			Image = "/example-apps/ptyxis.webp",
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
			Image = "/example-apps/wezterm.webp",
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
			Image = "/example-apps/windows-terminal.webp",
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