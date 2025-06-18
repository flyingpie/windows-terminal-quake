namespace Wtq.Configuration;

public static class HotkeyOptionsExtensions
{
	public static bool HasHotkey(this ICollection<HotkeyOptions> hotkeys, KeySequence sequence)
	{
		Guard.Against.Null(hotkeys);

		// TODO: hk == sequence (requires overloading equals operator).
		return hotkeys.Any(hk => hk.Sequence.Equals2(sequence));
	}
}