namespace Wtq.Services.WinForms.Native;

internal sealed class HotkeyEventArgs : EventArgs
{
	public HotkeyEventArgs(Keys key, KeyModifiers modifiers)
	{
		Key = key;
		Modifiers = modifiers;
	}

	public HotkeyEventArgs(nint hotkeyParam)
	{
		uint param = (uint)hotkeyParam.ToInt64();
		Key = (Keys)((param & 0xffff0000) >> 16);
		Modifiers = (KeyModifiers)(param & 0x0000ffff);
	}

	public Keys Key { get; }

	public KeyModifiers Modifiers { get; }
}