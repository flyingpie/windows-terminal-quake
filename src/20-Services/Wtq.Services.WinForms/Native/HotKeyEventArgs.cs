namespace Wtq.Services.WinForms.Native;

internal class HotKeyEventArgs : EventArgs
{
	public readonly Keys Key;
	public readonly KeyModifiers Modifiers;

	public HotKeyEventArgs(Keys key, KeyModifiers modifiers)
	{
		Key = key;
		Modifiers = modifiers;
	}

	public HotKeyEventArgs(nint HotKeyParam)
	{
		uint param = (uint)HotKeyParam.ToInt64();
		Key = (Keys)((param & 0xffff0000) >> 16);
		Modifiers = (KeyModifiers)(param & 0x0000ffff);
	}
}