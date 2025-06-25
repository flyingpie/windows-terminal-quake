namespace Wtq.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Keeps the file next to the KeyModifiers one.")]
public static class KeyCodeExtensions
{
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
			case KeyCode.LAlt:
			case KeyCode.RAlt:
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
			case KeyCode.LControl:
			case KeyCode.RControl:
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
			case KeyCode.NumPad0:
			case KeyCode.NumPad1:
			case KeyCode.NumPad2:
			case KeyCode.NumPad3:
			case KeyCode.NumPad4:
			case KeyCode.NumPad5:
			case KeyCode.NumPad6:
			case KeyCode.NumPad7:
			case KeyCode.NumPad8:
			case KeyCode.NumPad9:
			case KeyCode.Multiply:
			case KeyCode.Subtract:
			case KeyCode.Divide:
			case KeyCode.Add:
			case KeyCode.Decimal:
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
			case KeyCode.LShift:
			case KeyCode.RShift:
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
			case KeyCode.LSuper:
			case KeyCode.RSuper:
				return true;

			default:
				return false;
		}
	}
}