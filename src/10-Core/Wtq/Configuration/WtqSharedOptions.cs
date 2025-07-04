using Gn = Wtq.WtqConstants.Settings.GroupNames;
using Wc = Wtq.Configuration;
using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Configuration;

/// <summary>
/// Options that are available both in global- and per-app options.
/// </summary>
[Display(Name = ":material-cogs: Shared options")]
public abstract class WtqSharedOptions : IValidatableObject
{
	#region 2000 - Process

	/// <summary>
	/// <para>
	/// How WTQ should get to an instance of a running app.
	/// </para>
	/// <para>
	/// I.e. whether to start an app instance if one cannot be found.
	/// </para>
	/// </summary>
	[DefaultValue(Wc.AttachMode.FindOrStart)]
	[Display(GroupName = Gn.Process, Name = "Attach mode")]
	[JsonPropertyOrder(2005)]
	public AttachMode? AttachMode { get; set; }

	#endregion

	#region 3000 - Behavior

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.
	/// </summary>
	[DefaultValue(false)]
	[Display(GroupName = Gn.Behavior, Name = "Always on top")]
	[JsonPropertyOrder(3001)]
	public bool? AlwaysOnTop { get; set; }

	/// <summary>
	/// Whether the app should be toggled off when another app gets focus.
	/// </summary>
	[DefaultValue(Wc.HideOnFocusLost.Always)]
	[Display(GroupName = Gn.Behavior, Name = "Hide on focus lost")]
	[JsonPropertyOrder(3002)]
	public HideOnFocusLost? HideOnFocusLost { get; set; }

	/// <summary>
	/// When to show the app window icon on the taskbar.
	/// </summary>
	[DefaultValue(Wc.TaskbarIconVisibility.AlwaysHidden)]
	[Display(GroupName = Gn.Behavior, Name = "Taskbar icon visibility")]
	[JsonPropertyOrder(3003)]
	public TaskbarIconVisibility? TaskbarIconVisibility { get; set; }

	/// <summary>
	/// <para>
	/// Make the window see-through (applies to the entire window, including the title bar).
	/// </para>
	/// <para>
	/// 0 (invisible) - 100 (opaque).
	/// </para>
	/// </summary>
	[DefaultValue(100)]
	[Display(GroupName = Gn.Behavior)]
	[ExampleValue(80)]
	[JsonPropertyOrder(3004)]
	public int? Opacity { get; set; }

	#endregion

	#region 4000 - Position

	/// <summary>
	/// Horizontal screen coverage, as a percentage.
	/// </summary>
	[DefaultValue(95f)]
	[Display(GroupName = Gn.Position, Name = "Horizontal screen coverage", Prompt = "Percentage")]
	[JsonPropertyOrder(4001)]
	public float? HorizontalScreenCoverage { get; set; }

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.
	/// </summary>
	[DefaultValue(Wc.HorizontalAlign.Center)]
	[Display(GroupName = Gn.Position, Name = "Horizontal align")]
	[JsonPropertyOrder(4002)]
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <summary>
	/// Vertical screen coverage as a percentage (0-100).
	/// </summary>
	[DefaultValue(95f)]
	[Display(GroupName = Gn.Position, Name = "Vertical screen coverage", Prompt = "Percentage")]
	[JsonPropertyOrder(4003)]
	public float? VerticalScreenCoverage { get; set; }

	/// <summary>
	/// How much room to leave between the top of the app window and the top of the screen, in pixels.
	/// </summary>
	[DefaultValue(0f)]
	[Display(GroupName = Gn.Position, Name = "Vertical offset", Prompt = "In pixels")]
	[Range(0, 1000)]
	[JsonPropertyOrder(4004)]
	public float? VerticalOffset { get; set; }

	/// <summary>
	/// <para>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.
	/// </para>
	/// <para>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.
	/// </para>
	/// <para>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </para>
	/// </summary>
	[DefaultCollectionValue([Above, Below, Left, Right])] // TODO: Doesn't work yet. We're using WtqConstants.DefaultOffScreenLocations for now.
	[Display(GroupName = Gn.Position, Name = "Off-screen locations")]
	[JsonPropertyOrder(4005)]
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

	#endregion

	#region 5000 - Monitor

	/// <summary>
	/// Which monitor to preferably drop the app.
	/// </summary>
	[DefaultValue(Wc.PreferMonitor.WithCursor)]
	[Display(GroupName = Gn.Monitor, Name = "Prefer monitor")]
	[JsonPropertyOrder(5001)]
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// <para>
	/// If <strong>PreferMonitor</strong> is set to <strong>AtIndex</strong>, this setting determines what monitor to choose.
	/// </para>
	/// <para>
	/// Zero based, e.g. 0, 1, etc.
	/// </para>
	/// </summary>
	[DefaultValue(0)]
	[Display(GroupName = Gn.Monitor, Name = "Monitor index")]
	[Range(0, 10)]
	[JsonPropertyOrder(5002)]
	public int? MonitorIndex { get; set; }

	#endregion

	#region 6000 - Animation

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	[DefaultValue(250)]
	[Display(GroupName = Gn.Animation, Name = "Animation duration", Prompt = "In milliseconds")]
	[Range(0, 1000)]
	[JsonPropertyOrder(6001)]
	public int? AnimationDurationMs { get; set; }

	/// <summary>
	/// The animation type to use when toggling on an application.
	/// </summary>
	[DefaultValue(AnimationType.EaseOutQuart)]
	[Display(GroupName = Gn.Animation, Name = "Animation type (toggle ON)")]
	[JsonPropertyOrder(6003)]
	public AnimationType? AnimationTypeToggleOn { get; set; }

	/// <summary>
	/// The animation type to use when toggling off an application.
	/// </summary>
	[DefaultValue(AnimationType.EaseInQuart)]
	[Display(GroupName = Gn.Animation, Name = "Animation type (toggle OFF)")]
	[JsonPropertyOrder(6004)]
	public AnimationType? AnimationTypeToggleOff { get; set; }

	#endregion

	#region 7000 - EventHooks

	/// <summary>
	/// Execute a program when some event occurs.
	/// </summary>
	[Display(GroupName = Gn.Events, Name = "Event hooks")]
	[JsonPropertyOrder(7001)]
	public ICollection<WtqAppEventHookOptions> EventHooks { get; set; }
		= [];

	#endregion

	#region Validation

	/// <summary>
	/// Convenience property.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	[JsonIgnore]
	public bool IsValid => !this.Validate().Any();

	/// <summary>
	/// Convenience property to make binding from the GUI easier.
	/// </summary>
	[DisplayFlags(IsVisible = false)]
	[JsonIgnore]
	public IEnumerable<ValidationResult> ValidationResults => this.Validate();

	public IEnumerable<ValidationResult> Validate(ValidationContext context)
	{
		// TODO: Validate properties from this (WtqSharedOptions) class.

		foreach (var v in OnValidate(context))
		{
			yield return v;
		}
	}

	protected virtual IEnumerable<ValidationResult> OnValidate(ValidationContext context) =>
		[];

	#endregion
}