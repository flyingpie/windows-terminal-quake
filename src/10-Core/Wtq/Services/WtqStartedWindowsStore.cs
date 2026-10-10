using System.Text.Json;

namespace Wtq.Services;

/// <inheritdoc cref="IWtqStartedWindowsStore"/>
public sealed class WtqStartedWindowsStore(
	IPlatformService platform)
	: IWtqStartedWindowsStore
{
	private const string FileName = "wtq.state.json";

	private static readonly JsonSerializerOptions JsonOpts = new()
	{
		WriteIndented = true,
	};

	private readonly ILogger _log = Log.For<WtqStartedWindowsStore>();
	private readonly IPlatformService _platform = Guard.Against.Null(platform);
	private readonly Lock _lock = new();

	private State? _state;

	/// <inheritdoc/>
	public string? GetWindowId(string appName)
	{
		Guard.Against.NullOrWhiteSpace(appName);

		lock (_lock)
		{
			return Load().StartedWindows.GetValueOrDefault(appName);
		}
	}

	/// <inheritdoc/>
	public void SetWindowId(string appName, string windowId)
	{
		Guard.Against.NullOrWhiteSpace(appName);
		Guard.Against.NullOrWhiteSpace(windowId);

		lock (_lock)
		{
			var state = Load();

			if (state.StartedWindows.TryGetValue(appName, out var existing) && existing == windowId)
			{
				return;
			}

			state.StartedWindows[appName] = windowId;

			Save(state);
		}
	}

	/// <inheritdoc/>
	public void RemoveWindowId(string appName)
	{
		Guard.Against.NullOrWhiteSpace(appName);

		lock (_lock)
		{
			var state = Load();

			if (!state.StartedWindows.Remove(appName))
			{
				return;
			}

			Save(state);
		}
	}

	private string PathToStateFile =>
		Path.Combine(_platform.PathToWtqConfDir, FileName);

	private State Load()
	{
		if (_state != null)
		{
			return _state;
		}

		try
		{
			if (Fs.Inst.FileExists(PathToStateFile))
			{
				_state = JsonSerializer.Deserialize<State>(Fs.Inst.ReadAllText(PathToStateFile), JsonOpts);

				// The deserializer creates a dictionary with the default (case-sensitive) comparer, re-wrap to keep app names case-insensitive.
				if (_state != null)
				{
					_state.StartedWindows = new Dictionary<string, string>(_state.StartedWindows, StringComparer.OrdinalIgnoreCase);
				}
			}
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could not read state file at '{Path}', starting with empty state: {Message}", PathToStateFile, ex.Message);
		}

		return _state ??= new State();
	}

	private void Save(State state)
	{
		try
		{
			Fs.Inst.WriteAllText(PathToStateFile, JsonSerializer.Serialize(state, JsonOpts));
		}
		catch (Exception ex)
		{
			_log.LogWarning(ex, "Could not write state file at '{Path}': {Message}", PathToStateFile, ex.Message);
		}
	}

	/// <summary>
	/// Contents of the state file.
	/// </summary>
	private sealed class State
	{
		/// <summary>
		/// Per app name, the id of the window that WTQ started for it.
		/// </summary>
		public Dictionary<string, string> StartedWindows { get; set; } = new(StringComparer.OrdinalIgnoreCase);
	}
}
