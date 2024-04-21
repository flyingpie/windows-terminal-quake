namespace Wtq.Configuration;

/// <summary>
/// Defines a combination of a <see cref="Keys"/> value, with optional <see cref="KeyModifiers"/>, used for mapping a shortcut to an action.
/// </summary>
public class HotKeyOptions
{
	public Keys Key { get; set; }

	public KeyModifiers Modifiers { get; set; }
}