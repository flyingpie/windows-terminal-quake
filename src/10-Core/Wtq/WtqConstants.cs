using System.Runtime.InteropServices;

namespace Wtq;

public static class WtqConstants
{
	public static string AppVersion { get; }
		= typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

	public static Uri DocumentationUrl { get; }
		= new("https://wtq.flyingpie.nl");

	public static Uri GitHubUrl { get; }
		= new("https://www.github.com/flyingpie/windows-terminal-quake");

	/// <summary>
	/// Returns a list of <see cref="AnimationTypes"/> that include a <see cref="DisplayAttribute"/>, and de-duplicated.
	/// </summary>
	public static ICollection<AnimationType> AnimationTypes { get; } = Enum
		.GetValues<AnimationType>()
		.Select(k => new
		{
			Description = k.GetAttribute<AnimationType, DisplayAttribute>(),
			Key = k,
		})
		.Where(k => k.Description != null)
		.Select(k => k.Key)
		.Distinct()
		.ToArray();

	/// <summary>
	/// Returns a list of <see cref="Keys"/> that include a <see cref="DisplayAttribute"/>, and de-duplicated.
	/// </summary>
	public static ICollection<Keys> CommonKeys { get; } = Enum
		.GetValues<Keys>()
		.Select(k => new
		{
			Description = k.GetAttribute<Keys, DisplayAttribute>(),
			Key = k,
		})
		.Where(k => k.Description != null)
		.Select(k => k.Key)
		.Distinct()
		.ToArray();

	public static ICollection<WtqAppExample> AppExamples { get; } =
	[
		// Windows Terminal
		new()
		{
			Title = "Windows Terminal",
			Description = "Windows Terminal",
			Os = [ OSPlatform.Windows, ],
			Link = new("https://learn.microsoft.com/en-us/windows/terminal/install"),
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
			Os = [ OSPlatform.Windows, ],
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-5.1"),
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
			Os = [ OSPlatform.Windows, ], // On Linux, terminal emulator and shell are more separate than on Windows.
			Link = new("https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.4"),
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
			Description = "The cross-platform PowerShell, (version 6 and up, a.k.a. 'PowerShell Core').",
			Os = [ OSPlatform.Windows, ], // On Linux, terminal emulator and shell are more separate than on Windows.
			Link = new("https://wezfurlong.org/wezterm/index.html"),
			Factory = () => new()
			{
				Name = "Wezterm",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "",
			},
		},

		// Kitty
		new()
		{
			Title = "Kitty",
			Description = "The cross-platform PowerShell, (version 6 and up, a.k.a. 'PowerShell Core').",
			Os = [ OSPlatform.Windows, ],
			Link = new("https://sw.kovidgoyal.net/kitty/"),
			Factory = () => new()
			{
				Name = "Wezterm",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "",
			},
		},

		// Windows Explorer
		new()
		{
			Title = "Windows Explorer",
			Description = "Windows explorer, the default file browser on Windows.",
			Os = [ OSPlatform.Windows, ], // On Linux, terminal emulator and shell are more separate than on Windows.
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
			Description = "",
			Os = [ OSPlatform.Windows, ],
			Link = new("https://doublecommander.com"),
			Factory = () => new()
			{
				Name = "Double Commander",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "",
			},
		},

		// Q-Dir
		new()
		{
			Title = "Q-Dir",
			Description = "",
			Os = [ OSPlatform.Windows, ],
			Link = new("http://q-dir.com/"),
			Factory = () => new()
			{
				Name = "Double Commander",
				Hotkeys = [new() { Modifiers = KeyModifiers.Control, Key = Keys.D1, }],
				FileName = "",
			},
		},
	];

	private static readonly Random _r = new();

	public static string GetRandomPlaceholderName() => PlaceholderNames[_r.Next(0, PlaceholderNames.Length)];

	private static readonly string[] PlaceholderNames =
	[
		"Akira",
		"Ambassador",
		"Andromeda",
		"Antares",
		"Apollo",
		"Archer",
		"Bradbury",
		"Cardenas",
		"Centaur",
		"Challenger",
		"Cheyenne",
		"Chimera",
		"Columbia",
		"Constellation",
		"Constitution",
		"Crossfield",
		"Daedalus",
		"Danube",
		"Defiant",
		"Deneva",
		"Dreadnought",
		"Einstein",
		"Engle",
		"Erewhon",
		"Excelsior",
		"Freedom",
		"Freedom",
		"Galaxy",
		"Galen",
		"Hokule'a",
		"Hoover",
		"Intrepid",
		"Istanbul",
		"Korolev",
		"Luna",
		"Magee",
		"Malachowski",
		"Mediterranean",
		"Merced",
		"Merian",
		"Miranda",
		"Mulciber",
		"Nebula",
		"New Orleans",
		"Niagara",
		"Nimitz",
		"Norway",
		"Nova",
		"NX",
		"Oberth",
		"Odyssey",
		"Olympic",
		"Peregrine",
		"Prometheus",
		"Renaissance",
		"Rigel",
		"Saber",
		"Sequoia",
		"Shepard",
		"Shuttlecraft",
		"Sovereign",
		"Soyuz",
		"Springfield",
		"Steamrunner",
		"Surak",
		"Sydney",
		"Theophrastus",
		"Undetermined",
		"Universe",
		"Vesta",
		"Walker",
		"Wambundu",
		"Wells",
		"Yellowstone",
		"Yorkshire",
		"Zodiac",
	];

}

public class WtqAppExample
{
	public required string Title { get; init; }

	public required string Description { get; init; }

	public required Func<WtqAppOptions> Factory { get; init; }

	public required ICollection<OSPlatform> Os { get; init; }

	public Uri? Link { get; set; }

	public string? Image { get; set; }

	public string? Icon { get; set; }
}