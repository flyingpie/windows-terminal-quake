using System.Text;

namespace Wtq.Services.KWin.Input;

[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "MvdO: Naming convention.")]
public static class KeySequenceExtensions
{
	private static readonly ILogger _log = Log.For(typeof(KeySequenceExtensions));

	public static string ToKWinString(this KeySequence sequence)
	{
		var sb = new StringBuilder();

		// Determine whether the "Shift" key is required.
		// I.e., with sequence "Shift+F1", it _is_ required, as the actual key isn't affected by the shift.
		// But with "Ctrl+Shift+1", it _is not_ required, as it will be bound as "Ctrl+@" (on US keyboards).
		var mod = sequence.Modifiers;
		if (sequence.IsShiftImplied())
		{
			_log.LogInformation("Key sequence '{Sequence}' contains explicit SHIFT, which is implied by character '{Char}', omitting from KWin shortcut", sequence, sequence.KeyChar);

			mod ^= KeyModifiers.Shift;
		}

		// Modifier
		if (mod != KeyModifiers.None)
		{
			sb.Append(mod.ToKWinString());
		}

		// Key char
		// KWin uses key characters, so prefer that one if we have it right in the settings.
		if (sequence.HasKeyChar)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(sequence.KeyChar.ToKWinString());
		}

		// Alternatively, if no key character is present, map the key code.
		// This is not a perfect method, as the character depends on the keyboard layout, which I haven't found a way to access yet.
		else if (sequence.HasKeyCode)
		{
			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			var keyCodeStr = sequence.KeyCode.ToKWinString() ?? sequence.KeyCode.ToString();
			if (!string.IsNullOrWhiteSpace(keyCodeStr))
			{
				sb.Append(keyCodeStr);
			}
		}

		return sb.ToString();
	}

	/// <summary>
	/// Converts 0 or more modifiers into a string that can be used for registration in KWin.
	/// </summary>
	private static string ToKWinString(this KeyModifiers modifiers)
	{
		var sb = new StringBuilder();

		var mods = new[]
		{
			KeyModifiers.Alt, KeyModifiers.Control, KeyModifiers.Shift, KeyModifiers.Super, KeyModifiers.Numpad,
		};

		foreach (var m in mods)
		{
			if (!modifiers.HasFlag(m))
			{
				continue;
			}

			if (sb.Length > 0)
			{
				sb.Append('+');
			}

			sb.Append(m.ToKWinStringSingle());
		}

		return sb.ToString();
	}

	private static string ToKWinStringSingle(this KeyModifiers modifiers)
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
			default:
				return string.Empty;
		}
	}

	/// <summary>
	/// Converts no- or 1 key into a string that can be used for registration in KWin.
	/// </summary>
	private static string? ToKWinString(this KeyCode keyCode)
	{
		// @formatter:off
		#pragma warning disable SA1025
		#pragma warning disable SA1027

		switch (keyCode)
		{
			// The Most Popular Keys
			case KeyCode.Escape:					return "Esc";
			case KeyCode.PrintScreen:				return "Print";
			case KeyCode.Return:					return "Esc";
			case KeyCode.Space:						return "Space";
			case KeyCode.Tab:						return "Tab";

			// Arrow keys
			case KeyCode.ArrowDown:					return "Down";
			case KeyCode.ArrowLeft:					return "Left";
			case KeyCode.ArrowRight:				return "Right";
			case KeyCode.ArrowUp:					return "Up";

			// Above arrow keys
			case KeyCode.PageDown:					return "PgDown";
			case KeyCode.PageUp:					return "PgUp";

			// Modifiers
			case KeyCode.AltLeft:					return "Alt";
			case KeyCode.AltRight:					return "Alt";
			case KeyCode.ControlLeft:				return "Control";
			case KeyCode.ControlRight:				return "Control";
			case KeyCode.ShiftLeft:					return "Shift";
			case KeyCode.ShiftRight:				return "Shift";
			case KeyCode.SuperLeft:					return "Meta";
			case KeyCode.SuperRight:				return "Meta";

			// Browser
			case KeyCode.BrowserBack:				return null; // TODO: Unmapped
			case KeyCode.BrowserForward:			return null; // TODO: Unmapped
			case KeyCode.BrowserRefresh:			return null; // TODO: Unmapped
			case KeyCode.BrowserStop:				return null; // TODO: Unmapped
			case KeyCode.BrowserSearch:				return null; // TODO: Unmapped
			case KeyCode.BrowserFavorites:			return null; // TODO: Unmapped
			case KeyCode.BrowserHome:				return null; // TODO: Unmapped

			// Volume
			case KeyCode.VolumeMute:				return "Volume Mute";	// TODO: Verify
			case KeyCode.VolumeDown:				return "Volume Down";	// TODO: Verify
			case KeyCode.VolumeUp:					return "Volume Up";		// TODO: Verify

			// Media
			case KeyCode.MediaNextTrack:			return "Media Next";
			case KeyCode.MediaPreviousTrack:		return "Media Previous";
			case KeyCode.MediaStop:					return "Media Stop";
			case KeyCode.MediaPlayPause:			return "Media Play";

			case KeyCode.None:
			default:
				return null;
		}

		#pragma warning restore SA1027
		#pragma warning restore SA1025

		// @formatter:off
	}

	private static string ToKWinString(this string keyChar)
	{
		return

			EnumUtils.TryParse<KeyCode>(keyChar, ignoreCase: true, out var code)

			// If we can parse it, return the KWin (Qt) name of the key code.
			? code.ToKWinString()

			// If we can't parse it, return the upper-cased version of the key character (cause letters appear as uppercase in shortcuts it seems).
			?? keyChar.ToUpperInvariant()
			: keyChar.ToUpperInvariant();
	}
}