using System.Runtime.InteropServices;

namespace Wtq;

public class WtqAppExamples
{
	public static ICollection<WtqAppExample> AppExamples { get; } =
	[

		// Terminals ////////////////////////////////////////

		// Windows Terminal
		new()
		{
			Title = "Windows Terminal",
			Description = "Windows Terminal",
			Image = "/example_apps/windows_terminal.jpg",
			Link = new("https://learn.microsoft.com/en-us/windows/terminal/install"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Windows Terminal",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "wt",
				ProcessName = "WindowsTerminal",
			},
		},

		// PowerShell <=5 (Windows-only)
		new()
		{
			Title = "PowerShell 5",
			Description = "The original PowerShell, (Windows-only).",
			Image = "/example_apps/powershell_5.jpg",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-5.1"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "PowerShell 5",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "powershell",
			},
		},

		// PowerShell >=6 (Cross-platform)
		new()
		{
			Title = "PowerShell Core",
			Description = "The cross-platform PowerShell, (version 6 and up, a.k.a. 'PowerShell Core').",
			Image = "/example_apps/powershell_core.png",
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "PowerShell Core",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "pwsh",
			},
		},

		// Wezterm
		new()
		{
			Title = "Wezterm",
			Description = "Cross-platform GPU-accelerated terminal emulator.",
			Image = "/example_apps/wezterm.png",
			Link = new("https://wezfurlong.org/wezterm/index.html"),
			Os = [OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
			Factory = () => new()
			{
				Name = "Wezterm",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Kitty
		new()
		{
			Title = "Kitty",
			Description = "Cross-platform, fast, feature-rich, GPU based terminal.",
			Image = "/example_apps/kitty.png",
			Link = new("https://sw.kovidgoyal.net/kitty/"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Wezterm",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Ptyxis
		new()
		{
			Title = "Ptyxis",
			Description = "Ptyxis is a terminal for GNOME that focuses on ease-of-use in a world of containers.",
			Image = "/example_apps/ptyxis.webp",
			Link = null,
			Os = [OSPlatform.Linux,],
			Factory = () => new()
			{
				Name = "Ptyxis",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Konsole
		new()
		{
			Title = "Konsole",
			Description = "The default console emulator for KDE.",
			Image = "/example_apps/konsole.png",
			Link = null,
			Os = [OSPlatform.Linux,],
			Factory = () => new()
			{
				Name = "Konsole",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// File Browsers ////////////////////////////////////

		// Windows Explorer
		new()
		{
			Title = "Windows Explorer",
			Description = "Windows explorer, the default file browser on Windows.",
			Image = "/example_apps/windows_explorer.png",
			Os = [OSPlatform.Windows,], // On Linux, terminal emulator and shell are more separate than on Windows.
			Factory = () => new()
			{
				Name = "Windows Explorer",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "pwsh",
			},
		},

		// Double Commander
		new()
		{
			Title = "Double Commander",
			Description = "Midnight Commander-inspired file manager.",
			Image = "/example_apps/double_commander.png",
			Link = new("https://doublecommander.com"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Double Commander",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Q-Dir
		new()
		{
			Title = "Q-Dir",
			Description = "File manager with support for multiple panes, featuring lots of customization.",
			Image = "/example_apps/q_dir.png",
			Link = new("http://q-dir.com/"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Double Commander",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Dolphin
		new()
		{
			Title = "Dolphin",
			Description = "The default file manager for KDE.",
			Image = "/example_apps/dolphin.jpg",
			Link = null,
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Dolphin",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Misc /////////////////////////////////////////////

		// Spotify
		new()
		{
			Title = "Spotify",
			Description = "Music streaming service.",
			Image = "/example_apps/spotify.jpg",
			Link = new("https://www.spotify.com/us/download/"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Spotify",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Discord
		new()
		{
			Title = "Discord",
			Description = "Chat service.",
			Image = "/example_apps/discord.png",
			Link = new("https://discord.com/"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Discord",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// KeePassXC
		new()
		{
			Title = "KeePassXC",
			Description = "Password manager.",
			Image = "/example_apps/keepassxc.png",
			Link = new("https://keepassxc.org/"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "KeePassXC",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// Process Explorer
		new()
		{
			Title = "Process Explorer",
			Description = "Task manager on steroids.",
			Image = "/example_apps/process_explorer.jpg",
			Link = new("https://learn.microsoft.com/en-us/sysinternals/downloads/process-explorer"),
			Os = [OSPlatform.Windows,],
			Factory = () => new()
			{
				Name = "Process Explorer",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},

		// System Monitor (KDE)
		new()
		{
			Title = "System Monitor",
			Description = "Default task manager for KDE.",
			Image = "/example_apps/system_monitor.jpg",
			Link = null,
			Os = [OSPlatform.Linux,],
			Factory = () => new()
			{
				Name = "System Monitor",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "TODO",
			},
		},
	];
}