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

	public static bool IsAlt(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.LAltKey:
			case KeyCode.RAltKey:
				return true;

			default:
				return false;
		}
	}

	public static bool IsControl(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.LControlKey:
			case KeyCode.RControlKey:
				return true;

			default:
				return false;
		}
	}

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

	public static bool IsShift(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.LShiftKey:
			case KeyCode.RShiftKey:
				return true;

			default:
				return false;
		}
	}

	public static bool IsSuper(this KeyCode key)
	{
		switch (key)
		{
			case KeyCode.LSuperKey:
			case KeyCode.RSuperKey:
				return true;

			default:
				return false;
		}
	}
}