namespace Wtq.Configuration;

public static class HotkeyOptionsExtensions
{
	public static bool HasHotkey(this ICollection<HotkeyOptions> hotkeys, KeySequence sequence) =>
		Guard.Against.Null(hotkeys).Any(hk => hk.Sequence == sequence);
}