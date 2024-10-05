namespace Wtq.Services.WinForms;

internal static class Mapping
{
	internal static Configuration.Keys ToWtqKeys(this System.Windows.Forms.Keys key)
	{
		return (Configuration.Keys)key;
	}

	internal static Configuration.KeyModifiers ToWtqKeyModifiers(this Native.KeyModifiers mask)
	{
		return (Configuration.KeyModifiers)mask;
	}
}