namespace Wtq.Services.Win32.Extensions;

public static class ProcessExtensions
{
	public static bool TryGetHasExited(this Process process, out bool hasExited)
	{
		return process.TryGetProperty(nameof(process.HasExited), p => p.HasExited, out hasExited);
	}

	public static bool TryGetId(this Process process, out int id)
	{
		return process.TryGetProperty(nameof(process.Id), p => p.Id, out id);
	}

	public static bool TryGetMainModuleFileName(this Process process, out string? fileName)
	{
		return process.TryGetProperty(nameof(Process.MainModule.FileName), p => p.MainModule?.FileName, out fileName);
	}

	public static bool TryGetMainModuleName(this Process process, out string? mainModuleName)
	{
		return process.TryGetProperty(nameof(process.MainModule.ModuleName), p => p.MainModule?.ModuleName, out mainModuleName);
	}

	public static bool TryGetMainWindowHandle(this Process process, out nint mainWindowHandle)
	{
		return process.TryGetProperty(nameof(process.MainWindowHandle), p => p.MainWindowHandle, out mainWindowHandle);
	}

	public static bool TryGetMainWindowTitle(this Process process, out string? mainWindowTitle)
	{
		return process.TryGetProperty(nameof(process.MainWindowTitle), p => p.MainWindowTitle, out mainWindowTitle);
	}

	public static bool TryGetProcessName(this Process process, out string? processName)
	{
		return process.TryGetProperty(nameof(process.ProcessName), p => p.ProcessName, out processName);
	}

	private static bool TryGetProperty<TResult>(this Process process, string name, Func<Process, TResult> accessor, out TResult? value)
	{
		Guard.Against.Null(process);
		Guard.Against.Null(accessor);

		value = default;

		try
		{
			value = accessor(process);
			return true;
		}
		catch
		{
			// TODO: Log.
		}

		return false;
	}
}