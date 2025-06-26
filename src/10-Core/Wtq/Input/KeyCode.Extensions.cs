namespace Wtq.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Keeps the file next to the KeyModifiers one.")]
public static class KeyCodeExtensions
{
	public static KeyModifiers AsModifier(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.AltLeft:
			case KeyCode.AltRight:
				return KeyModifiers.Alt;

			case KeyCode.ControlLeft:
			case KeyCode.ControlRight:
				return KeyModifiers.Control;

			case KeyCode.ShiftLeft:
			case KeyCode.ShiftRight:
				return KeyModifiers.Shift;

			case KeyCode.SuperLeft:
			case KeyCode.SuperRight:
				return KeyModifiers.Super;

			default:
				return KeyModifiers.None;
		}
	}

	/// <summary>
	/// Returns whether the specified <paramref name="key"/> is a modifier (Alt, Control, Shift or Super).
	/// </summary>
	public static bool IsModifier(this KeyCode key)
	{
		return key.IsAlt() || key.IsControl() || key.IsShift() || key.IsSuper();
	}

	/// <summary>
	/// Whether this is the ALT key (either left or right).
	/// </summary>
	public static bool IsAlt(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.AltLeft:
			case KeyCode.AltRight:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// Whether this is the CONTROL key (either left or right).
	/// </summary>
	public static bool IsControl(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.ControlLeft:
			case KeyCode.ControlRight:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// Whether this key is part of the numpad/keypad.
	/// </summary>
	public static bool IsNumpad(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.Numpad0:
			case KeyCode.Numpad1:
			case KeyCode.Numpad2:
			case KeyCode.Numpad3:
			case KeyCode.Numpad4:
			case KeyCode.Numpad5:
			case KeyCode.Numpad6:
			case KeyCode.Numpad7:
			case KeyCode.Numpad8:
			case KeyCode.Numpad9:
			case KeyCode.NumpadMultiply:
			case KeyCode.NumpadSubtract:
			case KeyCode.NumpadDivide:
			case KeyCode.NumpadAdd:
			case KeyCode.NumpadDecimal:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// Whether this is the SHIFT key (either left or right).
	/// </summary>
	public static bool IsShift(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.ShiftLeft:
			case KeyCode.ShiftRight:
				return true;

			default:
				return false;
		}
	}

	/// <summary>
	/// Whether this is the SUPER (or META or WINDOWS) key (either left or right).
	/// </summary>
	public static bool IsSuper(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.SuperLeft:
			case KeyCode.SuperRight:
				return true;

			default:
				return false;
		}
	}
}