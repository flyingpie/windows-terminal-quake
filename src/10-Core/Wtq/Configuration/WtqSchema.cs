using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Wtq.Configuration;

/// <summary>
/// Used to generate a JSON schema file based on the <see cref="WtqOptions"/> class.
/// </summary>
public static class WtqSchema
{
	private static readonly ILogger _log = Log.For(typeof(WtqSchema));

	/// <summary>
	/// Writes a "wtq.schema.json" file next to the specified <paramref name="pathToWtqJson"/>.
	/// </summary>
	public static void WriteFor(string pathToWtqJson)
	{
		Guard.Against.NullOrWhiteSpace(pathToWtqJson);

		if (!File.Exists(pathToWtqJson))
		{
			_log.LogWarning("Path to settings file '{Path}' does not exist", pathToWtqJson);
			return;
		}

		var dir = Path.GetDirectoryName(pathToWtqJson)!;
		var dst = Path.Combine(dir, "wtq.schema.json");

		_log.LogInformation("Writing settings schema to '{Path}'", dst);

		try
		{
			File.WriteAllText(dst, GenerateSchema());
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error writing settings schema to path '{Path}': {Message}", dst, ex.Message);
		}
	}

	public static string GenerateSchema() =>
		JsonSchema.FromType<WtqOptions>(
				new SystemTextJsonSchemaGeneratorSettings()
				{
					SerializerOptions =
					{
						Converters =
						{
							new JsonStringEnumConverter(),
						},
					},
				})
			.ToJson(Formatting.Indented);
}