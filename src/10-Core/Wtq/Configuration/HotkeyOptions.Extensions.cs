namespace Wtq.Configuration;

public static class HotkeyOptionsExtensions
{
	public static bool HasHotkey(this ICollection<HotkeyOptions> hotkeys, Keys key, KeyModifiers modifiers)
	{
		Guard.Against.Null(hotkeys);

		return hotkeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}
}