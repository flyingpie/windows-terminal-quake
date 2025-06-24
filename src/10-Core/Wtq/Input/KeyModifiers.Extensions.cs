namespace Wtq.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Keeps the file next to the KeyModifiers one.")]
public static class KeyModifiersExtensions
{
	public static bool HasAlt(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Alt);

	public static bool HasControl(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Control);

	public static bool HasNumpad(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Numpad);

	public static bool HasShift(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Shift);

	public static bool HasSuper(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Super);
}