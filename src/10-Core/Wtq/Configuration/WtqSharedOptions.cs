using System.Linq.Expressions;
using System.Reflection;
using Wc = Wtq.Configuration;

namespace Wtq.Configuration;

public class WtqSharedOptions
{
	#region Animation

	private int? _animationDurationMsSwitchingApps;

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	[DisplayName("Animation duration (ms)")]
	[DefaultValue(250)]
	public int? AnimationDurationMs { get; set; }

	/// <summary>
	/// How many frames per second the animation should be.<br/>
	/// Note that this may not be hit if moving windows takes too long, hence "target" fps.<br/>
	/// Must be between 5 and 120, to prevent issues that can arise with values that are too low or too high.<br/>
	/// Defaults to 40.
	/// </summary>
	[DisplayName("Animation target FPS")]
	[DefaultValue(40)]
	public int? AnimationTargetFps { get; set; }

	/// <summary>
	/// How long the animation should take, in milliseconds, when switching between 2 WTQ-attached applications.<br/>
	/// This is a separate value, to prevent having 2 animation cycles stack, (one for toggling off the previous app, one for toggling on the next app).
	/// Defaults to <see cref="AnimationDurationMs"/> / 2.
	/// </summary>
	[JsonIgnore]
	public int AnimationDurationMsWhenSwitchingApps
	{
		get => (int)Math.Round(GetAnimationDurationMs() * .5f);
	}

	public float GetAnimationDurationMs()
		=> AnimationDurationMs ?? GetDefaultValue<float>(() => AnimationDurationMs);

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling on an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseOutQuart"/>.
	/// </summary>
	[DisplayName("TODO")]
	[DefaultValue(AnimationType.EaseOutQuart)]
	public AnimationType? AnimationTypeToggleOn { get; set; }

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling off an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseInQuart"/>.
	/// </summary>
	[DefaultValue(AnimationType.EaseInQuart)]
	public AnimationType? AnimationTypeToggleOff { get; set; }

	#endregion

	#region Behavior

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.<br/>
	/// Defaults to "false".
	/// </summary>
	[DefaultValue(false)]
	public bool? AlwaysOnTop { get; set; }

	/// <summary>
	/// Whether the app should be toggled out when another app gets focus.<br/>
	/// Defaults to "true".
	/// </summary>
	[DefaultValue(Wc.HideOnFocusLost.Always)]
	public HideOnFocusLost? HideOnFocusLost { get; set; }

	/// <summary>
	/// Make the window see-through (applies to the entire window, including the title bar).<br/>
	/// 0 (invisible) - 100 (opaque).<br/>
	/// Defaults to "100".
	/// </summary>
	[DefaultValue(100)]
	public int? Opacity { get; set; }

	/// <summary>
	/// When to show the terminal window icon on the taskbar.<br/>
	/// "AlwaysHidden", "AlwaysVisible" or "WhenTerminalVisible".<br/>
	/// Defaults to "AlwaysHidden".
	/// </summary>
	[DefaultValue(Wc.TaskbarIconVisibility.AlwaysHidden)]
	public TaskbarIconVisibility? TaskbarIconVisibility { get; set; }

	#endregion

	#region Monitor

	/// <summary>
	/// If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.<br/>
	/// Zero based, eg. 0, 1, etc.<br/>
	/// Defaults to "0".
	/// </summary>
	[DefaultValue(0)]
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// What monitor to preferrably drop the app.<br/>
	/// "WithCursor" (default), "Primary" or "AtIndex".
	/// </summary>
	[DefaultValue(Wc.PreferMonitor.WithCursor)]
	public PreferMonitor? PreferMonitor { get; set; }

	#endregion

	#region Position

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.<br/>
	/// Defaults to <see cref="HorizontalAlign.Center"/>.
	/// </summary>
	[DefaultValue(Wc.HorizontalAlign.Center)]
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <summary>
	/// Horizontal screen coverage, as a percentage.<br/>
	/// Defaults to "100".
	/// </summary>
	[DefaultValue(95f)]
	public float? HorizontalScreenCoverage { get; set; }

	/// <summary>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.<br/>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br/>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </summary>
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

	/// <summary>
	/// How much room to leave between the top of the terminal and the top of the screen, in pixels.<br/>
	/// Defaults to "0".
	/// </summary>
	[DefaultValue(0f)]
	public float? VerticalOffset { get; set; }

	/// <summary>
	/// Vertical screen coverage as a percentage (0-100).<br/>
	/// Defaults to "100".
	/// </summary>
	[DefaultValue(95f)]
	public float? VerticalScreenCoverage { get; set; }

	#endregion

	#region Process

	/// <summary>
	///	<p>The paragraph. With <strong>words</strong> and <em>empahisized</em> ones.</p>
	/// <p>Another paragraph<br/>
	/// With line breaks.</p>
	/// </summary>
	[DefaultValue(Wc.AttachMode.FindOrStart)]
	[JsonPropertyOrder(14)]
	public AttachMode? AttachMode { get; set; }

	#endregion

	public static TValue? GetDefaultValue<TValue>(Expression<Func<object?>> expr)
	{
		Guard.Against.Null(expr);

		var val = expr.GetMemberInfo().GetCustomAttribute<DefaultValueAttribute>()?.Value;

		if (val != null)
		{
			return (TValue)val;
		}

		return default;
	}
}