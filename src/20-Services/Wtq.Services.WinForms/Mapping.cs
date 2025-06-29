namespace Wtq.Services.WinForms;

internal static class Mapping
{
	internal static Input.KeyCode ToWtqKeys(this System.Windows.Forms.Keys key)
	{
		return (Input.KeyCode)key;
	}

	internal static Input.KeyModifiers ToWtqKeyModifiers(this Native.KeyModifiers mask)
	{
		return (Input.KeyModifiers)mask;
	}
}