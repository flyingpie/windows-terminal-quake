using System.Text;

namespace Wtq.Services.KWin.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Naming convention.")]
public static class KeySequenceExtensions
{
	public static string ToKWinString(this KeySequence sequence)
	{
		var sb = new StringBuilder();

		// TODO: Determine whether the "Shift" key is required.
		// I.e., with sequence "Shift+F1", it _is_ required, as the actual key isn't affected by the shift.
		// But with "Ctrl+Shift+1", it _is not_ required, as it will be bound as "Ctrl+@" (on US keyboards).

		// Modifier
		if (sequence.Modifiers != KeyModifiers.None)
		{
			sb.Append(sequence.Modifiers.ToKWinString());
		}

		// Key char
		// KWin uses key characters, so prefer that one if we have it right in the settings.
		if (sequence.HasKeyChar)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(sequence.KeyChar);
		}

		// Alternatively, if no key character is present, map the key code.
		// This is not a perfect method, as the character depends on the keyboard layout, which I haven't found a way to access yet.
		else if (sequence.HasKeyCode)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(sequence.KeyCode.ToKWinString());
		}

		return sb.ToString();
	}

	/// <summary>
	/// Converts 0 or more modifiers into a string that can be used for registration in KWin.
	/// </summary>
	private static string ToKWinString(this KeyModifiers modifiers)
	{
		switch (modifiers)
		{
			case KeyModifiers.Control:
				return "Ctrl";
			case KeyModifiers.Alt:
				return "Alt";
			case KeyModifiers.Shift:
				return "Shift";
			case KeyModifiers.Super:
				return "Meta";
			case KeyModifiers.Numpad:
				return "Num";

			case KeyModifiers.None:
			case KeyModifiers.NoRepeat:
			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// Converts 0 or 1 key into a string that can be used for registration in KWin.
	/// </summary>
	private static string ToKWinString(this KeyCode keyCode)
	{
		return "";
	}

	private static string ToKWinString(this string keyChar)
	{
		return null;
	}
}