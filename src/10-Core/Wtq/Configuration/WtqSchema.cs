using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Wtq.Configuration;

public static class WtqSchema
{
	private static readonly ILogger _log = Log.For(typeof(WtqSchema));

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
			var schema = JsonSchema.FromType<WtqOptions>(
				new SystemTextJsonSchemaGeneratorSettings()
				{
					SerializerOptions =
					{
						Converters =
						{
							new JsonStringEnumConverter(),
						},
					},
				});

			var schemaData = schema.ToJson(Formatting.Indented);

			File.WriteAllText(dst, schemaData);
		}
		catch (Exception ex)
		{
			_log.LogError(ex, "Error writing settings schema to path '{Path}': {Message}", dst, ex.Message);
		}
	}
}