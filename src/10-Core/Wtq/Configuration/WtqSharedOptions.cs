using Wc = Wtq.Configuration;

namespace Wtq.Configuration;

public class WtqSharedOptions
{
	#region Animation

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	[Display(Name = "Animation duration (ms)")]
	[DefaultValue(250)]
	[Range(0, 1000)]
	public int? AnimationDurationMs { get; set; }

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling on an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseOutQuart"/>.
	/// </summary>
	[Display(Name = "Animation type (toggle ON)")]
	[DefaultValue(AnimationType.EaseOutQuart)]
	public AnimationType? AnimationTypeToggleOn { get; set; }

	/// <summary>
	/// The <see cref="AnimationType"/> to use when toggling off an application.<br/>
	/// Defaults to <see cref="AnimationType.EaseInQuart"/>.
	/// </summary>
	[Display(Name = "Animation type (toggle OFF)")]
	[DefaultValue(AnimationType.EaseInQuart)]
	public AnimationType? AnimationTypeToggleOff { get; set; }

	#endregion

	#region Behavior

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.<br/>
	/// Defaults to "false".
	/// </summary>
	[DefaultValue(false)]
	[Display(Name = "Always on top")]
	public bool? AlwaysOnTop { get; set; }

	/// <summary>
	/// Whether the app should be toggled out when another app gets focus.<br/>
	/// Defaults to "true".
	/// </summary>
	[Display(Name = "Hide on focus lost")]
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
	[Display(Name = "Taskbar icon visibility")]
	[DefaultValue(Wc.TaskbarIconVisibility.AlwaysHidden)]
	public TaskbarIconVisibility? TaskbarIconVisibility { get; set; }

	#endregion

	#region Monitor

	/// <summary>
	/// If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.<br/>
	/// Zero based, e.g. 0, 1, etc.<br/>
	/// Defaults to "0".
	/// </summary>
	[Display(Name = "Monitor index")]
	[DefaultValue(0)]
	[Range(0, 10)]
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// Which monitor to preferably drop the app.<br/>
	/// "WithCursor" (default), "Primary" or "AtIndex".
	/// </summary>
	[Display(Name = "Prefer monitor")]
	[DefaultValue(Wc.PreferMonitor.WithCursor)]
	public PreferMonitor? PreferMonitor { get; set; }

	#endregion

	#region Position

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.<br/>
	/// Defaults to <see cref="HorizontalAlign.Center"/>.
	/// </summary>
	[Display(Name = "Horizontal align")]
	[DefaultValue(Wc.HorizontalAlign.Center)]
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <summary>
	/// Horizontal screen coverage, as a percentage.<br/>
	/// Defaults to "100".
	/// </summary>
	[Display(Name = "Horizontal screen coverage", Prompt = "Percentage")]
	[DefaultValue(95f)]
	public float? HorizontalScreenCoverage { get; set; }

	/// <summary>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.<br/>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br/>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </summary>
	[Display(Name = "Off-screen locations")]
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

	/// <summary>
	/// How much room to leave between the top of the terminal and the top of the screen, in pixels.<br/>
	/// Defaults to "0".
	/// </summary>
	[Display(Name = "Vertical offset", Prompt = "In pixels")]
	[DefaultValue(0f)]
	[Range(0, 1000)]
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
}