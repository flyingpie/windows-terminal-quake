namespace Wtq.Utils;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public sealed class DisplayFlagsAttribute : Attribute
{
	public bool IsVisible { get; set; } = true;
}