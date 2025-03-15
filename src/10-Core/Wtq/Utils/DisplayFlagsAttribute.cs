namespace Wtq.Utils;

/// <summary>
/// Sets additional metadata on properties and the like.
/// </summary>
[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class DisplayFlagsAttribute : Attribute
{
	/// <summary>
	/// Whether the value should be visible, e.g. in UI.
	/// </summary>
	public bool IsVisible { get; set; } = true;
}