namespace Wtq.WinForms.Native;

internal class HotKeyEventArgs : EventArgs
{
	public readonly Keys Key;
	public readonly KeyModifiers Modifiers;

	public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
	{
		Key = key;
		Modifiers = modifiers;
	}

	public HotKeyEventArgs(nint hotKeyParam)
	{
		uint param = (uint)hotKeyParam.ToInt64();
		Key = (Keys)((param & 0xffff0000) >> 16);
		Modifiers = (KeyModifiers)(param & 0x0000ffff);
	}
}