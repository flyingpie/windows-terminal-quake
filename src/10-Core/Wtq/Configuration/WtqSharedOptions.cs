using Gn = Wtq.WtqConstants.Settings.GroupNames;
using Wc = Wtq.Configuration;
using static Wtq.Configuration.OffScreenLocation;

namespace Wtq.Configuration;

/// <summary>
/// Options that are both in global <see cref="WtqOptions"/> and per-app <see cref="WtqAppOptions"/>.
/// </summary>
[Display(Name = ":material-cogs: Shared options")]
public abstract class WtqSharedOptions : IValidatableObject
{
	#region 2000 - Process

	/// <summary>
	/// How WTQ should get to an instance of a running app.<br/>
	/// I.e. whether to start an app instance if one cannot be found.
	/// </summary>
	[Display(GroupName = Gn.Process, Name = "Attach mode")]
	[DefaultValue(Wc.AttachMode.FindOrStart)]
	[JsonPropertyOrder(2005)]
	public AttachMode? AttachMode { get; set; }

	#endregion

	#region 3000 - Behavior

	/// <summary>
	/// Whether the app should always be on top of other windows, regardless of whether it has focus.
	/// </summary>
	[Display(GroupName = Gn.Behavior, Name = "Always on top")]
	[DefaultValue(false)]
	[JsonPropertyOrder(3001)]
	public bool? AlwaysOnTop { get; set; }

	/// <summary>
	/// Whether the app should be toggled off when another app gets focus.
	/// </summary>
	[Display(GroupName = Gn.Behavior, Name = "Hide on focus lost")]
	[DefaultValue(Wc.HideOnFocusLost.Always)]
	[JsonPropertyOrder(3002)]
	public HideOnFocusLost? HideOnFocusLost { get; set; }

	/// <summary>
	/// When to show the app window icon on the taskbar.
	/// </summary>
	[Display(GroupName = Gn.Behavior, Name = "Taskbar icon visibility")]
	[DefaultValue(Wc.TaskbarIconVisibility.AlwaysHidden)]
	[JsonPropertyOrder(3003)]
	public TaskbarIconVisibility? TaskbarIconVisibility { get; set; }

	/// <summary>
	/// Make the window see-through (applies to the entire window, including the title bar).<br/>
	/// 0 (invisible) - 100 (opaque).
	/// </summary>
	[Display(GroupName = Gn.Behavior)]
	[DefaultValue(100)]
	[ExampleValue(80)]
	[JsonPropertyOrder(3004)]
	public int? Opacity { get; set; }

	#endregion

	#region 4000 - Position

	/// <summary>
	/// Horizontal screen coverage, as a percentage.
	/// </summary>
	[Display(GroupName = Gn.Position, Name = "Horizontal screen coverage", Prompt = "Percentage")]
	[DefaultValue(95f)]
	[JsonPropertyOrder(4001)]
	public float? HorizontalScreenCoverage { get; set; }

	/// <summary>
	/// Where to position an app on the chosen monitor, horizontally.
	/// </summary>
	[Display(GroupName = Gn.Position, Name = "Horizontal align")]
	[DefaultValue(Wc.HorizontalAlign.Center)]
	[JsonPropertyOrder(4002)]
	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <summary>
	/// Vertical screen coverage as a percentage (0-100).
	/// </summary>
	[Display(GroupName = Gn.Position, Name = "Vertical screen coverage", Prompt = "Percentage")]
	[DefaultValue(95f)]
	[JsonPropertyOrder(4003)]
	public float? VerticalScreenCoverage { get; set; }

	/// <summary>
	/// How much room to leave between the top of the app window and the top of the screen, in pixels.
	/// </summary>
	[Display(GroupName = Gn.Position, Name = "Vertical offset", Prompt = "In pixels")]
	[DefaultValue(0f)]
	[Range(0, 1000)]
	[JsonPropertyOrder(4004)]
	public float? VerticalOffset { get; set; }

	/// <summary>
	/// When moving an app off the screen, WTQ looks for an empty space to move the window to.<br/>
	/// Depending on your monitor setup, this may be above the screen, but switches to below if another monitor exists there.<br/>
	/// By default, WTQ looks for empty space in this order: Above, Below, Left, Right.
	/// </summary>
	[Display(GroupName = Gn.Position, Name = "Off-screen locations")]
	[DefaultCollectionValue([Above, Below, Left, Right])] // TODO: Doesn't work yet. We're using WtqConstants.DefaultOffScreenLocations for now.
	[JsonPropertyOrder(4005)]
	public ICollection<OffScreenLocation>? OffScreenLocations { get; set; }

	#endregion

	#region 5000 - Monitor

	/// <summary>
	/// Which monitor to preferably drop the app.
	/// </summary>
	[Display(GroupName = Gn.Monitor, Name = "Prefer monitor")]
	[DefaultValue(Wc.PreferMonitor.WithCursor)]
	[JsonPropertyOrder(5001)]
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// If <strong>PreferMonitor</strong> is set to <strong>AtIndex</strong>, this setting determines what monitor to choose.<br/>
	/// Zero based, e.g. 0, 1, etc.
	/// </summary>
	[Display(GroupName = Gn.Monitor, Name = "Monitor index")]
	[DefaultValue(0)]
	[Range(0, 10)]
	[JsonPropertyOrder(5002)]
	public int? MonitorIndex { get; set; }

	#endregion

	#region 6000 - Animation

	/// <summary>
	/// How long the animation should take, in milliseconds.
	/// </summary>
	[Display(GroupName = Gn.Animation, Name = "Animation duration", Prompt = "In milliseconds")]
	[DefaultValue(250)]
	[Range(0, 1000)]
	[JsonPropertyOrder(6001)]
	public int? AnimationDurationMs { get; set; }

	/// <summary>
	/// The animation type to use when toggling on an application.
	/// </summary>
	[Display(GroupName = Gn.Animation, Name = "Animation type (toggle ON)")]
	[DefaultValue(AnimationType.EaseOutQuart)]
	[JsonPropertyOrder(6003)]
	public AnimationType? AnimationTypeToggleOn { get; set; }

	/// <summary>
	/// The animation type to use when toggling off an application.
	/// </summary>
	[Display(GroupName = Gn.Animation, Name = "Animation type (toggle OFF)")]
	[DefaultValue(AnimationType.EaseInQuart)]
	[JsonPropertyOrder(6004)]
	public AnimationType? AnimationTypeToggleOff { get; set; }

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