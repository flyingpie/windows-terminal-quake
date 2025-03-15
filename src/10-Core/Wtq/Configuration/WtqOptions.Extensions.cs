namespace Wtq.Configuration;

public static class WtqOptionsExtensions
{
	public static WtqAppOptions? GetAppOptionsByName(this WtqOptions global, string name)
	{
		Guard.Against.Null(global);
		Guard.Against.NullOrWhiteSpace(name);

		return global.Apps.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	public static WtqAppOptions GetAppOptionsByNameRequired(this WtqOptions global, string name)
	{
		Guard.Against.Null(global);

		return global.GetAppOptionsByName(name)
			?? throw new WtqException($"No options found for app with name '{name}'. These were found: {string.Join(", ", global.Apps.Select(a => a.Name))}.");
	}

	/// <inheritdoc cref="WtqOptions.AnimationTargetFps"/>
	public static int GetAnimationTargetFps(this WtqOptions global) =>
		Guard.Against.Null(global).AnimationTargetFps ?? AttrUtils.GetDefaultValueFor<int>(() => global.AnimationTargetFps);

	/// <inheritdoc cref="WtqOptions.ShowUiOnStart"/>
	public static bool GetShowUiOnStart(this WtqOptions global) =>
		Guard.Against.Null(global).ShowUiOnStart ?? AttrUtils.GetDefaultValueFor<bool>(() => global.ShowUiOnStart);
}