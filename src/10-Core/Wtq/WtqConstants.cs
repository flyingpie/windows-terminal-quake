using static Wtq.Configuration.OffScreenLocation;

namespace Wtq;

public static class WtqConstants
{
	public static string AppVersion { get; }
		= typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

	public static Uri DocumentationUrl { get; }
		= new("https://wtq.flyingpie.nl");

	public static Uri GitHubUrl { get; }
		= new("https://www.github.com/flyingpie/windows-terminal-quake");

	public static class Settings
	{
		public static class GroupNames
		{
			public const string Animation = nameof(Animation);
			public const string App = nameof(App);
			public const string Behavior = nameof(Behavior);
			public const string General = nameof(General);
			public const string Monitor = nameof(Monitor);
			public const string Position = nameof(Position);
			public const string Process = nameof(Process);
		}
	}

	/// <summary>
	/// The default off-screen locations are kept separate, to prevent arrays from merging during deserialization.
	/// We could do that by tweaking the JSON serializer, but that's way more complex.
	/// </summary>
	public static ICollection<OffScreenLocation> DefaultOffScreenLocations { get; } =
		[Above, Below, Left, Right];

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

	private static readonly Random _r = new();

	public static string GetRandomPlaceholderName() => _placeholderNames[_r.Next(0, _placeholderNames.Length)];

	private static readonly string[] _placeholderNames =
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

	public static class Sizes
	{
		public static readonly Size _1920x1080 = new(1920, 1080);
	}
}