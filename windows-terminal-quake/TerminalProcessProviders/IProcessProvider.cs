namespace WindowsTerminalQuake.TerminalProcessProviders;

public interface IProcessProvider
{
	Process Get();

	void OnExit(Action action);
}