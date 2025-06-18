namespace Wtq.Configuration;

public struct KeySequence
{
	/// <summary>
	/// The optional modifiers (ctrl, shift, etc.).
	/// </summary>
	public KeyModifiers Modifiers { get; set; }

	/// <summary>
	/// The pressed key (Q, 1, F1, etc.).
	/// </summary>
	public Keys? KeyCode { get; set; }

	public string? KeyChar { get; set; }

	public bool Equals2(KeySequence sequence) // TODO: Proper overloading
	{
		return Modifiers == sequence.Modifiers && (KeyCode == sequence.KeyCode || KeyChar == sequence.KeyChar);
	}
}