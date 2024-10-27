namespace Wtq.Services.KWin.Scripting;

public interface IKWinScriptService
{
	Task<KWinScript> LoadScriptAsync(string path);
}