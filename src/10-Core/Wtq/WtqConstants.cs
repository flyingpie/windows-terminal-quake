namespace Wtq;

public static class WtqConstants
{
	public static string AppVersion { get; }
		= typeof(WtqApp).Assembly.GetName().Version?.ToString() ?? "<unknown>";

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
}