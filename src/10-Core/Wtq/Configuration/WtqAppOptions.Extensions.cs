namespace Wtq.Configuration;

public static class WtqAppOptionsExtensions
{
	public static bool GetAlwaysOnTop(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<bool>(o => o.AlwaysOnTop, app, app.Global);
	}

	public static AttachMode GetAttachMode(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<AttachMode>(o => o.AttachMode, app, app.Global);
	}

	public static HideOnFocusLost GetHideOnFocusLost(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<HideOnFocusLost>(o => o.AttachMode, app, app.Global);
	}

	public static HorizontalAlign GetHorizontalAlign(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<HorizontalAlign>(o => o.HorizontalAlign, app, app.Global);
	}

	public static float GetHorizontalScreenCoverage(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<float>(o => o.HorizontalScreenCoverage, app, app.Global);
	}

	/// <summary>
	/// <see cref="WtqSharedOptions.HorizontalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public static float HorizontalScreenCoverageIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return app.GetHorizontalScreenCoverage() / 100f;
	}

	public static int GetOpacity(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<int>(o => o.Opacity, app, app.Global);
	}

	public static TaskbarIconVisibility GetTaskbarIconVisibility(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<TaskbarIconVisibility>(o => o.TaskbarIconVisibility, app, app.Global);
	}

	public static ICollection<OffScreenLocation> GetOffScreenLocations(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<ICollection<OffScreenLocation>>(o => o.OffScreenLocations, app, app.Global) ?? WtqConstants.DefaultOffScreenLocations;
	}

	public static float GetVerticalOffset(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<float>(o => o.VerticalOffset, app, app.Global);
	}

	public static float GetVerticalScreenCoverage(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<float>(o => o.VerticalScreenCoverage, app, app.Global);
	}

	/// <summary>
	/// <see cref="WtqSharedOptions.VerticalScreenCoverage"/> as an index (0 - 1).
	/// </summary>
	public static float VerticalScreenCoverageIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return app.GetVerticalScreenCoverage() / 100f;
	}

	public static int GetMonitorIndex(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<int>(o => o.MonitorIndex, app, app.Global);
	}

	public static PreferMonitor GetPreferMonitor(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<PreferMonitor>(o => o.PreferMonitor, app, app.Global);
	}

	public static int GetAnimationDurationMs(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<int>(o => o.AnimationDurationMs, app, app.Global);
	}

	#region 6000 - Animation

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="WtqSharedOptions.AnimationDurationMs"/> / 2.
	/// </summary>
	public static int GetAnimationDurationMsWhenSwitchingApps(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return (int)Math.Round(app.GetAnimationDurationMs() * .5f);
	}

	public static AnimationType GetAnimationTypeToggleOn(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<AnimationType>(o => o.AnimationTypeToggleOn, app, app.Global);
	}

	public static AnimationType GetAnimationTypeToggleOff(this WtqAppOptions app)
	{
		Guard.Against.Null(app);

		return Opts.Cascade<AnimationType>(o => o.AnimationTypeToggleOff, app, app.Global);
	}

	#endregion
}