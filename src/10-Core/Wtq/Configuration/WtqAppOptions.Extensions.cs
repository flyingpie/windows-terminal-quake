namespace Wtq.Configuration;

public static class WtqAppOptionsExtensions
{
	/// <inheritdoc cref="WtqSharedOptions.AlwaysOnTop"/>
	public static bool GetAlwaysOnTop(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<bool>(o => o.AlwaysOnTop, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.AnimationDurationMs"/>
	public static int GetAnimationDurationMs(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<int>(o => o.AnimationDurationMs, app, app.Global);
	}

	/// <summary>
	/// <see cref="WtqSharedOptions.AnimationDurationMs"/> / 2.
	/// </summary>
	public static int GetAnimationDurationMsWhenSwitchingApps(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return (int)Math.Round(app.GetAnimationDurationMs() * .5f);
	}

	/// <inheritdoc cref="WtqSharedOptions.AnimationTypeToggleOn"/>
	public static AnimationType GetAnimationTypeToggleOn(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<AnimationType>(o => o.AnimationTypeToggleOn, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.AnimationTypeToggleOff"/>
	public static AnimationType GetAnimationTypeToggleOff(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<AnimationType>(o => o.AnimationTypeToggleOff, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.AttachMode"/>
	public static AttachMode GetAttachMode(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<AttachMode>(o => o.AttachMode, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.HideOnFocusLost"/>
	public static HideOnFocusLost GetHideOnFocusLost(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<HideOnFocusLost>(o => o.HideOnFocusLost, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.HorizontalAlign"/>
	public static HorizontalAlign GetHorizontalAlign(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<HorizontalAlign>(o => o.HorizontalAlign, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.HorizontalScreenCoverage"/>
	public static float GetHorizontalScreenCoverage(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<float>(o => o.HorizontalScreenCoverage, app, app.Global);
	}

	/// <summary>
	/// <see cref="WtqSharedOptions.HorizontalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public static float GetHorizontalScreenCoverageIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return app.GetHorizontalScreenCoverage() / 100f;
	}

	/// <inheritdoc cref="WtqAppOptions.MainWindow"/>
	public static MainWindowState GetMainWindow(this WtqAppOptions app) =>
		Guard.Against.Null(app).MainWindow ?? AttrUtils.GetDefaultValueFor<MainWindowState>(() => app.MainWindow);

	/// <inheritdoc cref="WtqSharedOptions.MonitorIndex"/>
	public static int GetMonitorIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<int>(o => o.MonitorIndex, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.OffScreenLocations"/>
	public static ICollection<OffScreenLocation> GetOffScreenLocations(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<ICollection<OffScreenLocation>>(o => o.OffScreenLocations, app, app.Global) ?? WtqConstants.DefaultOffScreenLocations;
	}

	/// <inheritdoc cref="WtqSharedOptions.Opacity"/>
	public static int GetOpacity(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<int>(o => o.Opacity, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.PreferMonitor"/>
	public static PreferMonitor GetPreferMonitor(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<PreferMonitor>(o => o.PreferMonitor, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.Resize"/>
	public static Resizing GetResize(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<Resizing>(o => o.Resize, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.TaskbarIconVisibility"/>
	public static TaskbarIconVisibility GetTaskbarIconVisibility(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<TaskbarIconVisibility>(o => o.TaskbarIconVisibility, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.VerticalOffset"/>
	public static float GetVerticalOffset(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<float>(o => o.VerticalOffset, app, app.Global);
	}

	/// <inheritdoc cref="WtqSharedOptions.VerticalScreenCoverage"/>
	public static float GetVerticalScreenCoverage(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return OptionUtils.Cascade<float>(o => o.VerticalScreenCoverage, app, app.Global);
	}

	/// <summary>
	/// <see cref="WtqSharedOptions.VerticalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public static float GetVerticalScreenCoverageIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return app.GetVerticalScreenCoverage() / 100f;
	}
}