namespace Wtq.Utils;

/// <summary>
/// Used for marking properties that can be used to match/filter windows on.<br/>
/// Currently only used for the "Windows" page in the GUI, may want to extend this to the settings.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class CanBeMatchedOnAttribute : Attribute
{
}