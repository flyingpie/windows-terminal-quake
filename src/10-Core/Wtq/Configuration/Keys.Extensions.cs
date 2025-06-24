namespace Wtq.Configuration;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Keeps the file next to the KeyModifiers one.")]
public static class KeysExtensions
{
	// public static bool IsNumpad(this Keys key)
	// {
	// 	switch (key)
	// 	{
	// 		case Keys.NumPad0:
	// 		case Keys.NumPad1:
	// 		case Keys.NumPad2:
	// 		case Keys.NumPad3:
	// 		case Keys.NumPad4:
	// 		case Keys.NumPad5:
	// 		case Keys.NumPad6:
	// 		case Keys.NumPad7:
	// 		case Keys.NumPad8:
	// 		case Keys.NumPad9:
	// 		case Keys.Divide;
	// 		case Keys.Multiply:
	// 		case Keys.Subtract:
	// 		case Keys.Add:
	//
	// 		case "numpaddivide": return Keys.Divide;
	// 		case "numpadmultiply": return Keys.Multiply;
	// 		case "numpadsubtract": return Keys.Subtract;
	// 		case "numpadadd": return Keys.Add;
	// 		case "numpaddecimal": return Keys.Decimal;
	// 		case "numpadenter": return Keys.Enter;
	//
	// 			return true;
	// 		default:
	// 			return false;
	// 	}
	// }

	public static bool IsAlt(this Keys key)
	{
		switch (key)
		{
			case Keys.LAltKey:
			case Keys.RAltKey:
				return true;

			default:
				return false;
		}
	}

	public static bool IsControl(this Keys key)
	{
		switch (key)
		{
			case Keys.LControlKey:
			case Keys.RControlKey:
				return true;

			default:
				return false;
		}
	}

	public static bool IsModifier(this Keys key)
	{
		return key.IsAlt() || key.IsControl() || key.IsShift() || key.IsSuper();
	}

	public static bool IsShift(this Keys key)
	{
		switch (key)
		{
			case Keys.LShiftKey:
			case Keys.RShiftKey:
				return true;

			default:
				return false;
		}
	}

	public static bool IsSuper(this Keys key)
	{
		switch (key)
		{
			case Keys.LSuperKey:
			case Keys.RSuperKey:
				return true;

			default:
				return false;
		}
	}
}