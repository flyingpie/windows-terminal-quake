namespace Wtq.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Keeps the file next to the KeyModifiers one.")]
public static class KeyModifiersExtensions
{
	/// <summary>
	/// Whether ALT is part of the <paramref name="modifiers"/>.
	/// </summary>
	public static bool HasAlt(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Alt);

	/// <summary>
	/// Whether CONTROL is part of the <paramref name="modifiers"/>.
	/// </summary>
	public static bool HasControl(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Control);

	/// <summary>
	/// Whether NUMPAD is part of the <paramref name="modifiers"/>.
	/// </summary>
	public static bool HasNumpad(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Numpad);

	/// <summary>
	/// Whether SHIFT is part of the <paramref name="modifiers"/>.
	/// </summary>
	public static bool HasShift(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Shift);

	/// <summary>
	/// Whether SUPER (or META or WINDOWS) is part of the <paramref name="modifiers"/>.
	/// </summary>
	public static bool HasSuper(this KeyModifiers modifiers) =>
		modifiers.HasFlag(KeyModifiers.Super);
}