using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Wtq.Services.UI.Input;

namespace Wtq.Services.UI.Extensions;

public static class BlazorExtensions
{
	private static readonly ILogger _log = Log.For(typeof(BlazorExtensions));

	public static void ToModifiersAndKey(
		this KeyboardEventArgs ev,
		out KeyModifiers mod,
		out string? keyChar,
		out KeyCode keyCode)
	{
		Guard.Against.Null(ev);

		// Modifier
		mod = ToKeyModifiers(ev);

		// Key char
		keyChar = ev.Key;

		// Key code
		keyCode = ParseKeyCode(ev.Code);

		// If the key character is a special case (like "Tab", "F1", "PageUp", etc.), parse and normalize it as
		// a KeyCode, so we can easily use it as such during hotkey registration.
		// Note that this also includes " " (space).
		if (string.IsNullOrWhiteSpace(keyChar) || keyChar.Length > 1)
		{
			if (keyCode == KeyCode.None)
			{
				_log.LogWarning("Attempting to parse non-character key '{KeyChar}', but parsing failed. Keeping current value, but this may result in undesired behavior. Please file a bug at {Url}", keyChar, WtqConstants.GitHubUrl);
			}
		}

		var loc = ev.ToKeyLocation();

		// If this key comes from the numpad, add the "Numpad" modifier.
		if (loc == Html5DomKeyLocation.Numpad)
		{
			mod |= KeyModifiers.Numpad;
		}
	}

	/// <summary>
	/// Converts the numeric value of the "location" property to an <see cref="Html5DomKeyLocation"/>.
	/// </summary>
	/// <remarks>https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent#keyboard_locations.</remarks>
	private static Html5DomKeyLocation ToKeyLocation(this KeyboardEventArgs e)
	{
		switch ((int)e.Location)
		{
			case 0: return Html5DomKeyLocation.Standard;
			case 1: return Html5DomKeyLocation.Left;
			case 2: return Html5DomKeyLocation.Right;
			case 3: return Html5DomKeyLocation.Numpad;

			default: return Html5DomKeyLocation.Unknown;
		}
	}

	private static KeyModifiers ToKeyModifiers(KeyboardEventArgs ev)
	{
		var mod = KeyModifiers.None;

		if (ev.AltKey)
		{
			mod |= KeyModifiers.Alt;
		}

		if (ev.CtrlKey)
		{
			mod |= KeyModifiers.Control;
		}

		if (ev.ShiftKey)
		{
			mod |= KeyModifiers.Shift;
		}

		// Note that the "MetaKey" property doesn't seem to work on Photino+Linux.
		// This is handled through the KeyUp/KeyDown events.
		if (ev.MetaKey)
		{
			mod |= KeyModifiers.Super;
		}

		return mod;
	}

	/// <summary>
	/// Formats a key character such, that it can be parsed into a <see cref="KeyCode"/>.<br/>
	/// Useful for non-standard character stuff (like "Tab", "F1" and "PageUp"), so that we can correctly identify them when registering keys.
	/// </summary>

	/// <summary>
	/// <a href="https://www.toptal.com/developers/keycode/table"/>.
	/// </summary>
	private static KeyCode ParseKeyCode(string code)
	{
		// @formatter:off
		#pragma warning disable SA1025
		#pragma warning disable SA1027

		// Handle cases where the JS key code differs from the KeyCode enum.
		switch (code.ToLowerInvariant())
		{
			// Modifiers
			case "osleft":				return KeyCode.SuperLeft;
			case "osright":				return KeyCode.SuperRight;

			// Volume keys
			case "audiovolumeup":		return KeyCode.VolumeUp;
			case "audiovolumedown":		return KeyCode.VolumeDown;
			case "audiovolumemute":		return KeyCode.VolumeMute;

			case "backquote":			return KeyCode.OemTilde;
		}

		#pragma warning restore SA1027
		#pragma warning restore SA1025

		// Try to parse the code directly.
		if (Enum.TryParse<KeyCode>(code, ignoreCase: true, out var keyCode))
		{
			return keyCode;
		}

		// @formatter:off
		_log.LogWarning("Unknown key code '{Code}'", code);

		return KeyCode.None;
	}
}