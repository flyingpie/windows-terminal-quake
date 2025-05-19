// using Gn = Wtq.WtqConstants.Settings.GroupNames;
//
// namespace Wtq.Configuration;
//
// public class WtqAppEventHooksOptions
// 	: Dictionary<string, WtqAppEventHookOptions>
// {
// 	public WtqAppEventHooksOptions()
// 		: base(StringComparer.OrdinalIgnoreCase)
// 	{
// 	}
//
// 	[Display(GroupName = Gn.Events, Name = "App toggled on")]
// 	[JsonPropertyOrder(7002)]
// 	public WtqAppEventHookOptions AppToggledOn
// 	{
// 		get => this[nameof(AppToggledOn)];
// 		set => this[nameof(AppToggledOn)] = value;
// 	}
//
// 	public WtqAppEventHookOptions AppToggledOff
// 	{
// 		get => this[nameof(AppToggledOff)];
// 		set => this[nameof(AppToggledOff)] = value;
// 	}
//
// 	public WtqAppEventHookOptions AppSwitchedOn
// 	{
// 		get => this[nameof(AppSwitchedOn)];
// 		set => this[nameof(AppSwitchedOn)] = value;
// 	}
//
// 	public WtqAppEventHookOptions AppSwitchedOff
// 	{
// 		get => this[nameof(AppSwitchedOff)];
// 		set => this[nameof(AppSwitchedOff)] = value;
// 	}
// }